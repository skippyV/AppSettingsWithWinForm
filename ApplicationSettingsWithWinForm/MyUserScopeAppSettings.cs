
using System;
using System.Collections.ObjectModel;
using System.Configuration;

// https://learn.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-create-application-settings?view=netframeworkdesktop-4.8

namespace ApplicationSettingsWithWinForm
{
    internal class MyUserScopeAppSettings : ApplicationSettingsBase
    {

        [UserScopedSettingAttribute()]  // [ApplicationScopedSetting()]
        [DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsd=\"http://www.w3." +
            "org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
            "<string>scooby</string>\r\n</ArrayOfString>")]
        public System.Collections.Specialized.StringCollection DogNames
        {
            get
            {
                return (System.Collections.Specialized.StringCollection)this[nameof(DogNames)];
            }

            set
            {
                this[nameof(DogNames)] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int SelectedDogNameIndex
        { 
            get
            {
                return (int)this[nameof(SelectedDogNameIndex)];
            }

            set
            {
                this[nameof(SelectedDogNameIndex)] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("UserName:Zorga-Default")]
        public string UserName
        {
            get
            {
                return (String)this[nameof(UserName)];
            }
            set
            {
                this[nameof(UserName)] = value;
            }
        }


        // [ApplicationScopedSettingAttribute()] // does not appear applicable to ApplicationSettingsBase
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("420")]
        public int Port
        {
            get
            {
                return (int)this[nameof(Port)];
            }
            set
            {
                this[nameof(Port)] = value;
            }
        }

        // need to validate this
        public void AddDogName(string name)
        {
           // ((List<string>)this[nameof(DogNames)]).Add(name);
            DogNames.Add(name);
            SelectedDogNameIndex = DogNames.Count - 1;
        }

        public void DeleteDogName()
        {
            if (DogNames.Count > 0)
            {
                DogNames.RemoveAt(SelectedDogNameIndex);

                SelectedDogNameIndex--;
                if (SelectedDogNameIndex < 0 && DogNames.Count > 0)
                {
                    SelectedDogNameIndex = 0;
                }
            }            
        }
        
    }

    // https://www.enterprise-software-development.eu/posts/2019/10/22/how-to-implement-list-properties.html
    //public class Dogs
    //{
    //    readonly List<string> dogNames;
    //    public Dogs(List<string>? names = null)
    //    {
    //        dogNames = names?.ToList() ?? new List<string>();
    //        DogNames = dogNames.AsReadOnly();
    //    }

    //    public ReadOnlyCollection<string> DogNames { get; }

    //    public void AddDogName(string name) => dogNames.Add(name);

    //    public void RemoveDogName(string name) => dogNames.Remove(name);

    //    public void ClearDogNames() => dogNames.Clear();

    //    public string? FavoriteDogName { get; set; }

    //}
}
