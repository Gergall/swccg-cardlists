# SWCCG Card Lists

Code used to generate the static html cardlists such as (for example):
https://res.starwarsccg.org/cardlists/PremiereType.html
https://res.starwarsccg.org/cardlists/Set19Type.html
etc

## TOC

* <a href="#adding-a-new-set">Adding a New Set</a>
* <a href="#updating-the-page-html">Updating the Page HTML</a>
* <a href="#todo">To Do</a>


<a name="adding-a-new-set"></a>
## ADDING A NEW SET

So, a new set has been released and isn't showing up on the card lists page, and you got stuck figuring out how to add it?

You shouldn't need to edit the program for basic use such as simply adding a new set. If you are doing something more complex where you want to edit the program, use **Visual Studio 2022**.

1. Wherever you downloaded your local copy of this repository, navigate to the config file at \swccg-cardlists\SWListMaker\bin\Debug\SWListMaker.exe.config
  - NOTE: You might need to set Windows to show file extensions.  If extensions are hidden it might appear as SWListMaker.exe, with the .config hidden.

2. This SWListMaker.exe.config file has some settings we need to update (you can just edit it in Notepad):
  - maxVSet: Change the value to the set code of the highest numbered VSet. For example if the most recent VSet is 19, then the code for that is 219. It always takes the format of 2xx (so Set 20 code = 220, etc).
  - RepoPath: Adjust the filepath so it's accurate based on where you loaded your repository. If you keep your repo at D:\swccg\swccg-cardlists\ then that is your RepoPath.
  - DownloadLatestJSON: If set to Y, the program will first download the latest JSON card data before creating card lists. Y is the recommended setting.
  - JSONRemotePath: This is the path where the program will download the latest JSON files from. You will never need to change this unless we decide to host the JSON files elsewhere.

3. Now you can run SWListMaker.exe which is in the same directory as the config file you just edited.
  - When you run the program, a button appears. Click the button and wait a minute for the success message which says "Done! Cardlist files written to...", then close the program.
  - The updated card list HTML files you just generated can be found in the "cardlists" subfolder from the repo root. You can even open them in a web browser to see how they look.

4. The Premium pages are not auto-generated yet (sorry) but they are easy to update manually.
  - Premium.html, `PremiumRarity.html`, `PremiumName.html`, `PremiumType.html`
  - Just download/open the existing file, scroll down to the part where the Vsets are listed in the sidebar (around line 186) and insert a line to link to the new Vset.

5. Log into Amazon S3 and upload these HTML files, _you should have about 200 such files,_ to `/cardlists`
  - Yes, upload ALL ~200 of them, not just the new set. We need to update all the other pages sidebars

6. Issue an invalidation to clear the old pages out of cache
  - https://console.aws.amazon.com/cloudfront/v3/home?region=ca-central-1#/distributions/E4R02360UW5RJ/invalidations
  - Create Invalidation:
    ```
    /cardlists/*
    ```

7. Do some testing to see how the pages look. Good, hopefully!

8. You will find that the new Vset page is missing an image banner.  You'll need to create one and upload it.
  - Banner size is `735 x 93`
  - Upload banner to Amazon S3. File name and path must be exactly like this (using Set 19 as an example): `/cardlists/images/SET19_title.gif`


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

- UI Improvements
 - Better display of what the program is doing
 - Better display of what the current app settings are

- Update the `README.md` HowTo section after making some of these changes



### VISUAL
- Improve upon weird Set List headings that are black rectangles above each list. (One combined heading?)

- Better color scheme and other styles

- Better set banners: Set 19 is acceptable quality, Set 0 is not
  - Set banners don't require the name of the set but we may wish to make it more prominent below the banner



### CODE CLEANUP
- Update pages to use `logo_swccgpc.gif` from the `/images` folder instead of the `/cardlists` folder

- Clean up unused Amazon S3 files, especially entire contents of `/cardlists/images/starwars/` _(do we think they are being linked from anywhere?)_



### WISHLIST
- JSON Rulings and other info overlay on right side, or left side, or under card?
  - Or a tiny "More Info" link under the card that opens the card in Scomp

- Separate banners for Reflections 2 and Reflections 3

- Automatically update card lists
