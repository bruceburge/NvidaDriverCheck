# NvidaDriverCheck
LinqPad (http://www.linqpad.net/) script to check and see if new drivers have been released. 

 From http://www.geforce.com/whats-new/tag/drivers it Gets all the dates, with the CSS class date-display-single
 
 Will setup a folder in your user local app data folder, and create a text file, that text only contains the date string from the last time it checked.  It will then compare the local latest, to the one posted to determine if there has been a new driver published. 
 
 Uses Nlog, and overly logs everything. 
 
Not included, in this repo, but how its used curently.
Using linqpad's built in LPRUN the script is called weekily via a windows task, resulting in a DOS window telling me if there is likely a driver update. 

