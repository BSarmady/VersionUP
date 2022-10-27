# VersionUP
This is a tiny software development tool that increases version number of project automatically everytime it is built.

## How to use

### With visual studio
From "Project Properties" page, open "Build Event's" tab and put following line in "Pre-Build event command line"

```
$(ProjectDir)\VersionUp\VersionUp.exe "$(ProjectDir)\" "$(TargetPath)"
```
Copy `VersionUp.exe` file to `VersionUp` folder in root of your project.
If you are familiar with Visual Studio macros, you can modify the above line to match your development environment.


### From command line

If `VersionUp.exe` is in your path environment, run it as following

```
VersionUp "project folder" "Project output file"
```

- `Project folder` is where normally your `.csproj` file is which in most cases `AssemblyInfo.cs`  or `Properties\AssemblyInfo.cs` is
- `Project output file` is the full path to output of your project (dll or exe) file is which is normally `[Project folder]\bin\debug\somefile.dll` or something similar
 
## What does it do
Visual studio provides a very simple incremental versioning that can be achieved by placing * at the end of version number, however this is limited to just one star that increments only one number in your version.

Soon I found out this is not enough and version number generated doesn't provide any meaningful information so I decided to make my own

Version number created by this tool includes 4 parts `Major.Minor.Date.Build` 


- `Major` is assigned by developer and normally indicates this version has major changes such as feature changes, incompatibility with previous version or complete overhaul.
- `Minor` is assigned by developer and normally indicates there are bug fixes on for same version without major changes such as UI changes, addition or removal or features.
- `Date` is a 5 digit number that indicates when the app is built, the first 2 digits are year from 2000 and next 3 digits are day of the year. for example 22136 means the application was built on 2022 day 136 of year (16th May 2022)
- `Build` number is incremented with every build.

**Example** A version number created with this tool should look like 1.2.22136.1021 which means the application is version 1.2 and build 1021 and built on 16th May 2022.

**Note:** C# version will try to modify `AssemblyInfo.cs` directly and C++ version will try to modify `VER.RC` (standard C++ version resource)

**Note:** If you have followed the instruction from [How to use](https://github.com/BSarmady/VersionUP/blob/main/README.md#how-to-use) section above, this tool should run every time you build your application and automatically increment the version number. However if you have a CI\CD pipeline, you can include this with it so it only increment the version number on every release.

**Note:** There is a limitation on how many digits and how big the build number and date can be. These numbers are WORD (unsigned 16 bit numbers) hence they can go up to max 65535. For build number I decided to reset back to 0 after it reach 65535 but for date I don't touch it as I think if you are still using this app in year 2066 and you aren't retired or haven't found a better solution or microsoft still hasn't update the version number to be at least 32 bits then good luck to you.

