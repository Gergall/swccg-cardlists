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
  - RepoPath: Adjust the filepath so it's accurate based on where you loaded your repository. If you keep your repo at D:\swccg\swccg-cardlists\ then that is your RepoPath.
  - DownloadLatestJSON: If set to Y, the program will first download the latest JSON card data before creating card lists. Y is the recommended setting.
  - JSONRemotePath: This is the path where the program will download the latest JSON files from. You will never need to change this unless we decide to host the JSON files elsewhere.

3. Now you can run SWListMaker.exe which is in the same directory as the config file you just edited.
  - When you run the program, a button appears. Click the button and wait a minute for the success message which says "Done! Cardlist files written to...", then close the program.
  - The updated card list HTML files you just generated can be found in the "cardlists" subfolder from the repo root. You can even open them in a web browser to see how they look.

4. Log into Amazon S3 and upload these HTML files, _you should have over 200 such files,_ to `/cardlists`
  - Yes, upload ALL 200+ files, not just the new set. We need to update all the other pages sidebars

5. Issue an invalidation to clear the old pages out of cache
  - https://console.aws.amazon.com/cloudfront/v3/home?region=ca-central-1#/distributions/E4R02360UW5RJ/invalidations
  - Create Invalidation:
    ```
    /cardlists/*
    ```

6. Do some testing to see how the pages look. Good, hopefully!

7. You will find that the new Vset page is missing an image banner.  You'll need to create one and upload it.
  - Banner size is `735 x 93`
  - Upload banner to Amazon S3. File name and path must be exactly like this (using Set 19 as an example): `/cardlists/images/SET19_title.gif`

8. Check everything into Github.
  - Don't forget to add the new cardlist pages and add the banner image!


<a name="updating-the-page-html"></a>
## UPDATING THE PAGE HTML

 - `PageTemplate.txt` is the template. Updating this file (for example, adding text), will affect all pages (except the Premium page) produced by the program.
 - `PremiumTemplate.txt` is the template for the Premium page which is different from the rest.
 - `swccg2.css` stylesheet can be edited for some things like swapping colors etc.





<a name="todo"></a>
## TODO

### FUNCTIONAL
- UI Improvements
 - Better display of what the program is doing
 - Better display of what the current app settings are



### VISUAL
- Improve upon weird Set List headings that are black rectangles above each list. (One combined heading?)

- Better color scheme and other styles

- Better set banners: Set 19 is acceptable quality, Set 0 is not
  - Set banners don't require the name of the set but we may wish to make it more prominent below the banner



### CODE CLEANUP
- Clean up unused Amazon S3 files, especially entire contents of `/cardlists/images/starwars/` _(do we think they are being linked from anywhere?)_



### WISHLIST
- JSON Rulings and other info overlay on right side, or left side, or under card?
  - Or a tiny "More Info" link under the card that opens the card in Scomp

- Separate banners for Reflections 2 and Reflections 3

- Automatically update card lists

- More mobile friendly especially when user zooms in

- More intuitive buttons for pop-out and dismiss ("X out")