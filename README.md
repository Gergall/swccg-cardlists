# SWCCG Card Lists

Code used to generate the static html cardlists: https://res.starwarsccg.org/cardlists/Set19Type.html

## TOC

* <a href="#adding-a-new-set">Adding a New Set</a>
* <a href="#updating-the-page-html">Updating the Page HTML</a>
* <a href="#todo">To Do</a>


<a name="adding-a-new-set"></a>
## ADDING A NEW SET

So, a new set has been released and isn't showing up on the card lists page, and you got stuck figuring out how to add it?

1. As of **September 2022**, you have to edit the program to add a new set. Download and Install **Visual Studio 2019**.

2. Update the program by open the .sln file in Visual Studio 2019 and make these upates in Form1.cs:
  - In the `button1_click` function, update the `maxVSet` variable. Currently its `219`, indicating **Set 19**. To add **Set 20**, you'd change this to `220`.

3. The program also has some hard-coded file paths.
  - For example, `SWListMaker/Form1.cs` contains hard-coded: `D:\swccg\SWListMaker\cardlists\` references
  - `ProcessSet` function has 4 lines with hard-coded paths of where you want the HTML files to get dumped. Update to whatever works for you.
  - `BuildPage` function has 1 line with THREE paths in it, indicating the locations of `PagePart1.txt`, `PagePart2.txt`, and `PagePart3.txt`. Update ALL THREE accordingly.
  - `GetCardList` function has 1 line indicating where your JSON files are. Make sure you have the latest JSON files _(get from [swccg-card-json github](https://github.com/swccgpc/swccg-card-json))_ - `Dark.json`, `Light.json`, `DarkLegacy.json`, `LightLegacy.json`, and `sets.json`

4. Finally you can build the program with your changes (F6) and then run the program (F5).
  - When you run the program, a button appears. Click the button and wait a minute for the success message which says "File(s) Written"

5. The Premium pages are not auto-generated yet (sorry) but they are easy to update manually.
  - Premium.html, `PremiumRarity.html`, `PremiumName.html`, `PremiumType.html`
  - Just download/open the existing file, scroll down to the part where the Vsets are listed in the sidebar (around line 186) and insert a line to link to the new Vset.

6. Log into Amazon S3 and upload these HTML files, _you should have about 200 such files,_ to `/cardlists`
  - Yes, upload ALL ~200 of them, not just the new set. We need to update all the other pages sidebars

7. Issue an invalidation to clear the old pages out of cache
  - https://console.aws.amazon.com/cloudfront/v3/home?region=ca-central-1#/distributions/E4R02360UW5RJ/invalidations
  - Create Invalidation:
    ```
    /cardlists/*
    ```

8. Do some testing to see how the pages look. Good, hopefully!

9. You need to make the new Vset page doesn't have an image banner? 
  - Banner size is `735 x 93`
  - Upload banner to Amazon S3. File name and path must be exactly like this (using Set 20 as an example): `/cardlists/images/SET19_title.gif`


<a name="updating-the-page-html"></a>
## UPDATING THE PAGE HTML

 - `PagePart1.txt`, `PagePart2.txt`, and `PagePart3.txt` form a sort of template. Updating these files, _for example, adding text,_ will affect all pages produced by the program.
 - Additionally, updating `swccg2.css` will allow for simple things like swapping colors etc.





<a name="todo"></a>
## TODO

### FUNCTIONAL
- Program should automatically generate 4 "Reflections" pages that are simply identical to Reflections2 pages. Just in case there are any links out there pointing to "Reflections", we may as well keep it maintained.
  - `Reflections.html`, `ReflectionsName.html`, `ReflectionsRarity.html`, `ReflectionsType.html`

- Program should automatically generate the 4 "Premium" pages. This will be done by saving them in a template TXT file and using a wildcard for the sidebar that needs updating.
  - `Premium.html`, `PremiumRarity.html`, `PremiumName.html`, `PremiumType.html`
  - **Currently I have been updating the sidebar of the 4 premium pages manually**.

- Program should connect directly to the current JSON files online by downloading from GitHub instead of requiring a local copy that could be outdated.

- Update the `README.md` HowTo section after making some of these quality of life changes



### VISUAL
- Improve upon weird Set List headings that are black rectangles above each list. (One combined heading?)

- Better color scheme and other styles

- Better set banners: Set 19 is acceptable quality, Set 0 is not
  - Set banners don't require the name of the set but we may wish to make it more prominent below the banner



### CODE CLEANUP
- Replace hard-coded paths with config file variables

- Replace hard-coded `maxVSet` variable with config info, or by pulling from Sets.json

- Update the `README.md` file after making some of these quality of life changes

- Update pages to use `logo_swccgpc.gif` from the `/images` folder instead of the `/cardlists` folder

- Clean up unused Amazon S3 files, especially entire contents of `/cardlists/images/starwars/` _(do we think they are being linked from anywhere?)_



### WISHLIST
- JSON Rulings and other info overlay on right side, or left side, or under card?
  - Or a tiny "More Info" link under the card that opens the card in Scomp

- Separate banners for Reflections 2 and Reflections 3

- Automatically update card lists
