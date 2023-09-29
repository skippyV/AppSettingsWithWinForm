
For this project, it is my intent to use the Declarative approach to defining the 
settings via code. My focus is on read AND write of settings via code.
This exercise will attempt to help me better understand what I can do for future reference.

Everytime I poke my head into WinForm settings I have to relearn the same caveats.

This lesson started with looking at ApplicationSettingsBase and that class's allowed attributes
   ApplicationScopedSettingAttribute
   UserScopedSettingAttribute
I would like to have an example of both attributes working.

But only the UserScopedSettingAttribute appears relevant to the ApplicationSettingsBase derived class.

Then I derived from ConfigurationSection and got ConfigurationPropertyAttribute to work.

But still could not get an ApplicationScopedSettingAttribute to be created from code using
either ApplicationSettingsBase derived class or ConfigurationSection derived class.
  
 My brain expected that I'd just decorate the property with ApplicationScopedSettingAttribute, and then
 expect the Settings.Save() to write changes back to the config file (for the ApplicationSettingsBase approach)
  
 To manage (read/write) ApplicationScopedSettingAttribute settings from code, you can derive a class from
 ConfigurationSection class
 https://learn.microsoft.com/en-us/dotnet/api/system.configuration.configurationsection?view=dotnet-plat-ext-7.0&redirectedfrom=MSDN
  
From Hans Passant (https://stackoverflow.com/questions/453161/how-can-i-save-application-settings-in-a-windows-forms-application/453390#453390)
     The ApplicationSettings class doesn't support saving settings to the app.config file. 
     That's very much by design; applications that run with a properly secured user account (think Vista UAC) do not have 
     write access to the program's installation folder.
     You can fight the system with the ConfigurationManager class
 
This thread has posts stating one needs to do a ForceSave to write app settings back to file
https://stackoverflow.com/questions/1687321/how-do-i-get-around-application-scope-settings-being-read-only
 
In reflection on how I usually want my apps to work, I don't really need application level settings.
Just enough defaults to get the thing working and allow the user to change what they want.
Whereas the use case for app level settings would be more of a multi-user system wide need, for
example, where IT suport needs to update the service location. Then they can access the install
folder, manually tweak the .config file and tell the users to restart their apps.

I also wanteded to see how I can change the location directory of both config files 
  - ok, for application level settings - not sure how I would do this without creating a new AppDomain (too much), and
  the preferred way is to just reference another config file. Doing manual tweaks of the config file is easy but the
  intent of this project was to use attributes and code to generate all configs. But think doing the additional config
  file and its properties ALL with attributes, would be a bigger headache than I wanted to tackle - if not impossible. 
      https://stackoverflow.com/questions/4796801/moving-app-config-file-to-a-custom-location
  - as for user level settings, that would break the MS design which is driven by isolation, security, etc. So not
  going to worry about configuration paths for this project.
      https://stackoverflow.com/questions/2518772/how-to-change-net-user-settings-location
 
Wanted a default value for List<string> to be populated with but could not figure out how to
do that with DefaultSettingValueAttribute. So used the designer to see how what options it 
allows and the closest thing to a List allowed was the System.Collections.Specialized.StringCollection.
And the default value is set with an XML blob. Not as pretty as I'd hoped but works.

UPDATE - I'd thought I had the right incantations to always get the ApplicationSettingsBase.Save() method
to save to file, when I called Save(). But then as I slowly incorporated it into my NumberBoard winform 
project, it was not saving! Even though I used the Form.Load event handler for both projects, to do a 
save. However, I had not yet done any bindings in the NumberBoard project. Just instantiated the Settings
class and expected to see the defaults (or something) written to file. However, when I did an assignment 
to one of the Properties in the Settings class, the file was generated with that property's value.
So, there must be some lazy loading going on here where if the properties are not used in a binding or set
with a value, within code, then the Save method will not do anything. This had me scratching my head for 
quite a while. So now I just need to put in my bindings and then a file should be generated.

Note - this project uses Serilog for a couple logging statements.