# Me Too!
This is a simple(ish) tool to copy files to multiple places at once. 

## Before You Run
It requires some reasonably nasty admin priviledges to add / delete registry keys as the main interaction takes the form of a context menu addition. 

>I would suggest having a look at the RegistryBuilder class to make sure you're cool with what it's doing before running it.


## Building
There shouldn't be any special dependancies for the program. It's relatively simple in that it only uses built in C# functionality.
I'm using VS 2013 and so maybe this will work for you. It does need to be built exclusively for x64 and this is due to the registry locations changing dependent on x86/x64

>Obviously being a Winform / Context addition application there's no support or plan to support non-Windows OS

## Usage
There are 3 modes to the program,

1. No arguments / running the program directy is effectively an installation step, copies itself to WINDIR and adds Configure context menu (so do this as admin)
2. Providing the arguement 'configure' will open the Winform to set up the DIR linkages (called copysets)
3. Providing two arguements that correspond to a copyset and source file. The source file must reside within a DIR of the copyset


## Limitations
* Me Too! won't create folders for you
* You can not copy a folder
* You can not see the context addition unless you have opened the menu on a file
* You can not copy a source file to a copyset unless it exists in the copyset
* The interface is averagely polished
* If you delete Me Too! it will leave some registry keys lying around (check to source for the locations if you have run it and want to rid yourself of it)
* Running in VS will generate its own config location and so the copysets will be different that running it normally

>Some of these issues I plan to fix, some of them are a victim of the implementation.
