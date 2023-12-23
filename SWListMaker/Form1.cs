using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Configuration;
using System.Net;

namespace SWListMaker
{
    public partial class Form1 : Form
    {
        //Various file paths
        static string strRepoPath = ConfigurationManager.AppSettings["RepoPath"].ToString();
        static string strCardlistOutputPath = strRepoPath + @"cardlists\"; //card list output files go to the cardlists subfolder
        static string strJSONFilePath = strRepoPath + @"JSON\"; //JSON source files are in the JSON subfolder
        static string strPageTemplatePath = strRepoPath + "PageTemplate.txt"; //Page template file found in the root directory
        static string strPremiumTemplatePath = strRepoPath + "PremiumTemplate.txt"; //Page template file found in the root directory
        static string strJSONRemotePath = ConfigurationManager.AppSettings["JSONRemotePath"].ToString();

        //A yes/no setting that determines whether to use download the latest JSON from online
        static string strDownloadLatestJSON = ConfigurationManager.AppSettings["DownloadLatestJSON"].ToString();
        static bool blJSONHasBeenDownloaded = RefreshJSON();

        //Identify the highest numbered VSet by reading it from sets.json
        static int maxVSet = GetMaxVSet();

        //Get the left side menu links. Obviously, we needed to determine the maxVSet first (above)
        static string strVSETMENULINKS = GetVSETMENULINKS(maxVSet);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            textBox1.Text = "Working, please wait...";

            //Decipher Sets
            ProcessSet("1", "Premiere", "PREMIERE_title.gif", "Premiere");
            ProcessSet("2", "A New Hope", "ANEWHOPE_title.gif", "ANewHope");
            ProcessSet("3", "Hoth", "HOTH_title.gif", "Hoth");
            ProcessSet("4", "Dagobah", "DAGOBAH_title.gif", "Dagobah");
            ProcessSet("5", "Cloud City", "CC_title.gif", "CloudCity");
            ProcessSet("6", "Jabba's Palace", "JP_title.gif", "JabbasPalace");
            ProcessSet("7", "Special Edition", "SE_title.gif", "SpecialEdition");
            ProcessSet("8", "Endor", "ENDOR_title.gif", "Endor");
            ProcessSet("9", "Death Star II", "DS2_title.gif", "DeathStar2");
            ProcessSet("10", "Reflections II", "REF2_title.gif", "Reflections2");
            ProcessSet("11", "Tatooine", "TATOOINE_title.gif", "Tatooine");
            ProcessSet("12", "Coruscant", "CORUSCANT_title.gif", "Coruscant");
            ProcessSet("13", "Reflections III", "REF3_title.gif", "Reflections3");
            ProcessSet("14", "Theed Palace", "THEED_title.gif", "Theed");
            //Decipher Premium
            ProcessSet("101", "Premium", "PREMIUM_title.gif", "Premium");

            string strSet;
            int i;

            //Virtual Sets
            for (i = 200; i <= maxVSet; i++)
            {
                strSet = (i - 200).ToString();
                ProcessSet(i.ToString(), "Set " + strSet, "SET" + strSet + "_title.gif", "Set" + strSet);
            }
            ProcessSet("200d", "Set D", "SETD_title.gif", "SetD");
            ProcessSet("301", "Set P", "SETP_title.gif", "SetP");

            //Legacy Virtual Blocks
            for (i = 1001; i <= 1009; i++)
            {
                strSet = (i - 1000).ToString();
                ProcessSet(i.ToString(), "Virtual Block " + strSet, "v" + strSet + "-title.jpg", "VBlock" + strSet);
            }
            ProcessSet("1000d", "Virtual Block D", "vd_title.jpg", "VShields");

            ProcessSet("LEGACYMASTER", "Virtual Master", "VMaster-title.jpg", "VMaster");
            
            textBox1.Text = "Done! Cardlist files written to:" + System.Environment.NewLine + strCardlistOutputPath;
        }

        private void ProcessSet(string strSetID, string strSetName, string strBannerFile, string strOutputFile)
        {
            textBox1.Text = "Processing Set " + strSetID + "...";

            List<SWCard> LSCardResults = GetCardList(strSetID, "Light");
            List<SWCard> DSCardResults = GetCardList(strSetID, "Dark");

            string strPage;

            //Sorted By Rarity
            LSCardResults = SortByRarity(LSCardResults);
            DSCardResults = SortByRarity(DSCardResults);
            strPage = BuildPage(strSetName, strOutputFile, strBannerFile, LSCardResults, DSCardResults, "Rarity", false);
            File.WriteAllText(strCardlistOutputPath + strOutputFile+"Rarity.html", strPage);
            if (strOutputFile == "Reflections2") { File.WriteAllText(strCardlistOutputPath + "Reflections" + "Rarity.html", strPage); } //When we make a Reflections2 page, we also make an identical Reflections page

            //Sorted By Title (aka Name)
            LSCardResults = SortByTitle(LSCardResults);
            DSCardResults = SortByTitle(DSCardResults);
            strPage = BuildPage(strSetName, strOutputFile, strBannerFile, LSCardResults, DSCardResults, "Name", false);
            File.WriteAllText(strCardlistOutputPath + strOutputFile + "Name.html", strPage);
            if (strOutputFile == "Reflections2") { File.WriteAllText(strCardlistOutputPath + "Reflections" + "Name.html", strPage); } //When we make a Reflections2 page, we also make an identical Reflections page

            //Sorted By Type
            LSCardResults = SortByType(LSCardResults);
            DSCardResults = SortByType(DSCardResults);
            strPage = BuildPage(strSetName, strOutputFile, strBannerFile, LSCardResults, DSCardResults, "Type", true);
            File.WriteAllText(strCardlistOutputPath + strOutputFile + "Type.html", strPage);
            if (strOutputFile == "Reflections2") { File.WriteAllText(strCardlistOutputPath + "Reflections" + "Type.html", strPage); } //When we make a Reflections2 page, we also make an identical Reflections page

            //Plain Jane (same as Type, no need to re-sort)
            strPage = BuildPage(strSetName, strOutputFile, strBannerFile, LSCardResults, DSCardResults, "", true);
            File.WriteAllText(strCardlistOutputPath + strOutputFile + ".html", strPage);
            if (strOutputFile == "Reflections2") { File.WriteAllText(strCardlistOutputPath + "Reflections" + ".html", strPage); } //When we make a Reflections2 page, we also make an identical Reflections page
        }

        private string BuildPage(string strSetName, string strSetAbbr, string strBannerFile, List<SWCard> LSCards, List<SWCard> DSCards, string strSort, bool blTypeHeadings)
        {
            string previousCardType = ""; //used for Type Headings
            string strHTMLLS = "";
            string strHTMLDS = "";
            string tmpBackString = "";

            if (LSCards.Count == 0)
            {
                strHTMLLS += "<tr><td class='left' colspan='3'>No Light Side cards in " + strSetName + "</td></tr>";
            }
            foreach (SWCard currentCard in LSCards)
            {
                if (blTypeHeadings && currentCard.FrontType != previousCardType) //we need to print a heading before this card
                {
                    strHTMLLS += "<tr><td>&nbsp;</td><td class='type'> " + currentCard.FrontType.ToUpper() + " </td><td>&nbsp;</td></tr>" + System.Environment.NewLine;
                    previousCardType = currentCard.FrontType;
                }

                if (currentCard.BackTitle == "") //minor prep work for 2-sided cards
                    tmpBackString = "";
                else
                    tmpBackString = " / <a href='" + currentCard.BackImageUrl + "' target='_blank' onclick=\"return fnShowCard('" + currentCard.BackImageUrl + "'," + currentCard.Horizontal + ",'Light');\">" + currentCard.BackTitle + "</a>";

                //time to print a card
                strHTMLLS += "<tr><td class='center' style='width:31px'><img src='" + currentCard.GetIconFullPath() + "' height='21px' width='21px' alt=\"" + currentCard.TypeText + "\" title=\"" + currentCard.TypeText + "\" /></td><td class='left'><a href='" + currentCard.FrontImageUrl + "' target='_blank' onclick=\"return fnShowCard('" + currentCard.FrontImageUrl + "'," + currentCard.Horizontal + ",'Light');\">" + currentCard.FrontTitle + "</a>" + tmpBackString + "</td><td class='center' style='width:43px'>" + currentCard.Rarity + "</td></tr>" + System.Environment.NewLine;
            }

            if (DSCards.Count == 0)
            {
                strHTMLLS += "<tr><td class='left' colspan='3'>No Dark Side cards in " + strSetName + "</td></tr>";
            }
            foreach (SWCard currentCard in DSCards)
            {
                if (blTypeHeadings && currentCard.FrontType != previousCardType) //we need to print a heading before this card
                {
                    strHTMLDS += "<tr><td>&nbsp;</td><td class='type'> " + currentCard.FrontType.ToUpper() + " </td><td>&nbsp;</td></tr>" + System.Environment.NewLine;
                    previousCardType = currentCard.FrontType;
                }

                if (currentCard.BackTitle == "") //minor prep work for 2-sided cards
                    tmpBackString = "";
                else
                    tmpBackString = " / <a href='" + currentCard.BackImageUrl + "' target='_blank' onclick=\"return fnShowCard('" + currentCard.BackImageUrl + "'," + currentCard.Horizontal + ",'Dark');\">" + currentCard.BackTitle + "</a>";

                //time to print a card
                strHTMLDS += "<tr><td class='center' style='width:31px'><img src='" + currentCard.GetIconFullPath() + "' height='21px' width='21px' alt=\"" + currentCard.TypeText + "\" title=\"" + currentCard.TypeText + "\" /></td><td class='left'><a href='" + currentCard.FrontImageUrl + "' target='_blank' onclick=\"return fnShowCard('" + currentCard.FrontImageUrl + "'," + currentCard.Horizontal + ",'Dark');\">" + currentCard.FrontTitle + "</a>" + tmpBackString + "</td><td class='center' style='width:43px'>" + currentCard.Rarity + "</td></tr>" + System.Environment.NewLine;
            }

            //string strPage = File.ReadAllText(strPageTemplatePath + "PagePart1.txt") + strHTMLLS + File.ReadAllText(strPageTemplatePath + "PagePart2.txt") + strHTMLDS + File.ReadAllText(strPageTemplatePath + "PagePart3.txt");

            string strPage = "";
            if (strSetName == "Premium")
                strPage = File.ReadAllText(strPremiumTemplatePath);
            else
                strPage = File.ReadAllText(strPageTemplatePath);

            strPage = strPage.Replace("~~LSCARDLIST~~", strHTMLLS);
            strPage = strPage.Replace("~~DSCARDLIST~~", strHTMLDS);
            strPage = strPage.Replace("•", "&#8226;");
            strPage = strPage.Replace("~~SETABBR~~", strSetAbbr);
            strPage = strPage.Replace("~~SETNAME~~", strSetName);
            strPage = strPage.Replace("~~BANNERFILE~~", strBannerFile);
            strPage = strPage.Replace("~~VSETMENULINKS~~", strVSETMENULINKS); //It's important that this be done before ~~SORT~~ which is the next line
            strPage = strPage.Replace("~~SORT~~", strSort);

            if (strSort == "")
            {
                strPage = strPage.Replace("~~SORTWITHDEFAULTTYPE~~", "Type"); //We hardcode in the word "Type" instead of leaving blank
            }
            else
            {
                strPage = strPage.Replace("~~SORTWITHDEFAULTTYPE~~", strSort);
            }

            if (strSetName.IndexOf("Virtual Block") > -1 || strSetName == "Virtual Master") //We are building a legacy page
            {
                strPage = strPage.Replace("~~LEGACYWARNING~~", "<span style='color:#AA0000;'><strong>Legacy Set Alert</strong><br />Legacy Virtual Cards (also known as the \"Virtual Block\" sets) are from a previous era of Star Wars CCG (2002-2014) and are no longer valid for competitive play. You may view them below, but if you are planning to play SWCCG, please use the Decipher Sets and the current Virtual Sets.<br /><br /></span>");
                strPage = strPage.Replace("~~LEGACYLINK~~", "Hide");
                strPage = strPage.Replace("~~LEGACYDIV~~", "block");
            }
            else //We are building a normal page
            {
                strPage = strPage.Replace("~~LEGACYWARNING~~", "");
                strPage = strPage.Replace("~~LEGACYLINK~~", "Show");
                strPage = strPage.Replace("~~LEGACYDIV~~", "none");
            }

            return strPage;
        }

        private static int GetMaxVSet()
        {
            //this function reads sets.json and figures out the highest vset number.
            int maxVSet = 201; //201 is just a momentary placeholder value

            string jsonFilePath = strJSONFilePath + "sets.json";
            string strJSON = File.ReadAllText(jsonFilePath);
            string strCurrentID = "";
            int currentID = 201;

            List<JObject> dataItems = JsonConvert.DeserializeObject<List<JObject>>(strJSON);

            if (dataItems != null && dataItems.Count > 0)
            {
                foreach (JObject item in dataItems)
                {
                    strCurrentID = item["id"].ToString();
                    int.TryParse(strCurrentID, out currentID);

                    if (currentID < 300 && currentID > maxVSet) //vsets are in the 200s, so we igore anything 300 or higher
                        maxVSet = currentID;
                }
            }

            return maxVSet;
        }

        private static string GetVSETMENULINKS(int maxVSet)
        {
            int maxVSetFriendly = maxVSet - 200;
            string strResult = "";

            for (int i=0; i<=maxVSetFriendly; i++)
            {
                strResult += "           &nbsp;&nbsp;&gt <a class=\"smo\" href=\"https://res.starwarsccg.org/cardlists/Set" + i.ToString() + "~~SORT~~.html\">Set " + i.ToString() + "</a><br/>\n";
            }

            return strResult;
        }

        private List<SWCard> GetCardList(string theSet, string side)
        {
            //TO DO: Add code with special treatment if theSet==LEGACYMASTER

            string filename;
            //if (theSet == "1001" || theSet == "1002" || theSet == "1003" || theSet == "1004" || theSet == "1005" || theSet == "1006" || theSet == "1007" || theSet == "1008" || theSet == "1009" || theSet == "1000d" || theSet == "LEGACYMASTER")
            if (IsSetLegacy(theSet))
            {
                filename = side + "Legacy.json"; 
            }
            else
            {
                filename = side + ".json";
            }

            string jsonFilePath = strJSONFilePath + filename;

            string strJSON = File.ReadAllText(jsonFilePath);
            dynamic cardarray = JsonConvert.DeserializeObject(strJSON);
            var cards = cardarray.cards;

            string set, rarity, frontType, frontSubType, frontTitle, backType, backSubType, backTitle;

            List<SWCard> SWCardResults = new List<SWCard>();
            SWCard theNewCard;

            foreach (var card in cards)
            {
                set = card.set;
                //if (set == theSet)
                if ((set == theSet) || (theSet == "LEGACYMASTER" && IsSetLegacy(set)))
                {
                    rarity = card.rarity;
                    frontType = card.front.type;
                    frontSubType = card.front.subType;
                    frontTitle = card.front.title;

                    theNewCard = new SWCard();
                    theNewCard.Side = card.side;
                    theNewCard.Title = card.front.title;
                    theNewCard.FrontImageUrl = card.front.imageUrl;

                    if (card.back is null)
                    {
                        backType = "";
                        backSubType = "";
                        backTitle = "";
                        theNewCard.BackImageUrl = "";

                    }
                    else
                    {
                        backType = card.back.type;
                        backSubType = card.back.subType;
                        backTitle = card.back.title;
                        theNewCard.BackImageUrl = card.back.imageUrl;
                    }

                    theNewCard.setRarity(rarity);
                    theNewCard.setTitle(frontTitle, backTitle);
                    theNewCard.setType(frontType, frontSubType, backType, backSubType, frontTitle);
                    

                    SWCardResults.Add(theNewCard);
                }
            }

            return SWCardResults;

        }

        private List<SWCard> SortByRarity(List<SWCard> ListToSort)
        {
            ListToSort.Sort(delegate (SWCard x, SWCard y)
            {
                string xSort = x.RaritySort + x.Title.Replace("•", "").Replace("<", "").Replace(">", "");
                string ySort = y.RaritySort + y.Title.Replace("•", "").Replace("<", "").Replace(">", "");
                return xSort.CompareTo(ySort);
            });
            return ListToSort;
        }

        private List<SWCard> SortByTitle(List<SWCard> ListToSort)
        {
            ListToSort.Sort(delegate (SWCard x, SWCard y)
            {
                string xSort = x.Title.Replace("•", "").Replace("<", "").Replace(">","");
                string ySort = y.Title.Replace("•", "").Replace("<", "").Replace(">", "");
                return xSort.CompareTo(ySort);
            });
            return ListToSort;
        }

        private List<SWCard> SortByType(List<SWCard> ListToSort)
        {
            ListToSort.Sort(delegate (SWCard x, SWCard y)
            {
                string xSort = x.FrontType + x.Title.Replace("•", "").Replace("<", "").Replace(">", "");
                string ySort = y.FrontType + y.Title.Replace("•", "").Replace("<", "").Replace(">", "");
                return xSort.CompareTo(ySort);
            });
            return ListToSort;
        }

        private bool IsSetLegacy(string strSet)
        {
            if (strSet == "1001" || strSet == "1002" || strSet == "1003" || strSet == "1004" || strSet == "1005" || strSet == "1006" || strSet == "1007" || strSet == "1008" || strSet == "1009" || strSet == "1000d" || strSet == "LEGACYMASTER")
                return true;
            else
                return false;
        }

        private static bool RefreshJSON()
        {
            if (strDownloadLatestJSON == "Y")
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(strJSONRemotePath + "Dark.json", strJSONFilePath + "Dark.json");
                    client.DownloadFile(strJSONRemotePath + "DarkLegacy.json", strJSONFilePath + "DarkLegacy.json");
                    client.DownloadFile(strJSONRemotePath + "Light.json", strJSONFilePath + "Light.json");
                    client.DownloadFile(strJSONRemotePath + "LightLegacy.json", strJSONFilePath + "LightLegacy.json");
                    client.DownloadFile(strJSONRemotePath + "sets.json", strJSONFilePath + "sets.json");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string strSettings = "";
            strSettings += "Latest VSet identified as: Set " + maxVSet.ToString().Substring(1) + System.Environment.NewLine + System.Environment.NewLine;

            strSettings += "These are the current settings. To modify them, close program and edit the App.config file." + System.Environment.NewLine;
            strSettings += "Refresh JSON files with latest online copies? (Y/N): " + strDownloadLatestJSON + System.Environment.NewLine;
            if (strDownloadLatestJSON == "Y")
                strSettings += "Path to online JSON files: " + strJSONRemotePath + System.Environment.NewLine;
            strSettings += "Local repository (root folder): " + strRepoPath + System.Environment.NewLine;

            txtSettings.Text = strSettings;
        }
    }
}
