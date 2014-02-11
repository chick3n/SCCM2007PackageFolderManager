# ConfigMan Folder Manager

Application to manage package folders by setting distribution points on them. All packages under said folder will be adjusted to either add/remove its distribution points. Any distribution points added they will be flagged for a refresh.

To maintain each folders mappings you can use the console app (FolderManagerCLI) and add it to a scheduled task on the server. It will use the saved mappings file generated from the GUI to check and set a packages DP. 

#### Requirements

> .NET 4.0


## GUI

![Alt text](/GUI.png "GUI Screenshot")

Input the server name of your sccm site, and it will than grab a list of all folders, subsequent packages, and distribution points. Selecting a folder will show a list of DP's and from their you can check them on/off. Use the APPLY button to apply your changes to your server and save the mappings xml file.

## CLI

The console application takes 2 parameters, 1 is required. 1st parameter is location of the mappings file, the second parameter is the location where you want to save the log file.
