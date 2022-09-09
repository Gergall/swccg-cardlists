using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWListMaker
{
    class SWCard
    {
        public string Side { get; set; }
        public string Title { get; set; }
        public string FrontTitle { get; set; }
        public string BackTitle { get; set; }

        public string Rarity { get; set; }
        public string RaritySort { get; set; }

        public string FrontImageUrl { get; set; }
        public string FrontType { get; set; }
        public string FrontSubType { get; set; }

        public string BackImageUrl { get; set; }
        public string BackType { get; set; }
        public string BackSubType { get; set; }

        public string TypeIcon { get; set; }
        public string TypeText { get; set; }

        public int Horizontal { get; set; }

        public void setRarity(string theRarity)
        {
            this.Rarity = theRarity;

            switch (theRarity)
            {
                case "PM": RaritySort = "A"; break;
                case "UR": RaritySort = "B"; break;
                case "XR": RaritySort = "C"; break;
                case "R": RaritySort = "D"; break;
                case "R1": RaritySort = "E"; break;
                case "R2": RaritySort = "F"; break;
                case "U": RaritySort = "G"; break;
                case "U1": RaritySort = "H"; break;
                case "U2": RaritySort = "I"; break;
                case "C": RaritySort = "J"; break;
                case "C1": RaritySort = "K"; break;
                case "C2": RaritySort = "L"; break;
                case "C3": RaritySort = "M"; break;
                case "F": RaritySort = "N"; break;
                default: RaritySort = "_"; break;
            }
        }

        public void setTitle(string theFrontTitle, string theBackTitle)
        {
            if (theBackTitle == "")
            {
                this.FrontTitle = theFrontTitle;
                this.BackTitle = theBackTitle;
            }
            else
            {
                string[] titles = theFrontTitle.Split('/'); //If there is a back title, the front title will look like this "Watch Your Step / This Place Can Be A Little Rough"

                this.FrontTitle = titles[0].Trim();
                this.BackTitle = titles[1].Trim();
            }

        }

        public void setType(string theFrontType, string theFrontSubType, string theBackType, string theBackSubType, string frontTitle)
        {
            if (theFrontType.IndexOf("Jedi Test") > -1)
                theFrontType = "Jedi Test";

            this.FrontType = theFrontType;
            this.FrontSubType = theFrontSubType;
            this.BackType = theBackType;
            this.BackSubType = theBackSubType;

            if (theFrontSubType=="Site" || frontTitle == "•Executor" || frontTitle == "•Flagship Executor" || frontTitle == "•Maul's Double-Bladed Lightsaber" || frontTitle == "•Finalizer" || frontTitle == "•Finalizer (AI)")
            {
                this.Horizontal = 1;
            }
            else
            {
                this.Horizontal = 0;
            }

            switch (theFrontType)
            {
                case "Location":
                    this.TypeIcon = theFrontSubType.ToLower().Replace(" ", "_").Replace("/", "_").Replace("'", "") + ".gif";
                    this.TypeText = theFrontSubType;
                    break;
                case "Character":
                    this.TypeIcon = theFrontSubType.ToLower().Replace(" ", "_").Replace("/", "_").Replace("'", "") + ".gif";
                    this.TypeText = theFrontSubType;
                    break;
                case "Starship":
                    if (theBackType == "Vehicle") //flip falcon specific code!
                    {
                        this.TypeIcon = "starship_vehicle.gif";
                        this.TypeText = "Starship/Vehicle";
                    }
                    else
                    {
                        this.TypeIcon = theFrontType.ToLower().Replace(" ", "_").Replace("/", "_").Replace("'", "") + ".gif";
                        this.TypeText = theFrontType;
                    }
                    break;
                default:
                    this.TypeIcon = theFrontType.ToLower().Replace(" ", "_").Replace("/", "_").Replace("'", "") + ".gif";
                    this.TypeText = theFrontType;
                    break;
            }
        }
        public string GetIconFullPath()
        {
            if (this.Side == "Light")
                return "https://res.starwarsccg.org/cardlists/images/LS/" + this.TypeIcon;
            else
                return "https://res.starwarsccg.org/cardlists/images/DS/" + this.TypeIcon;
        }

    }
}
