
using Serilog;
using System.Collections;
using System.Configuration;
using System.Text;

namespace ApplicationSettingsWithWinForm
{
    public partial class Form1 : Form
    {
        private MyUserScopeAppSettings? UserScopedSettings;
        private MyAppScopeConfigSection? AppScopeCustomConfigSection;

        private Point ComboBoxLocation = new(30, 30);
        private readonly string ComboBoxName = "MyComboBox";

        private Point ListBoxLocation = new(190, 30);
        private readonly string ListBoxName = "MyListBox";

        private Point AddItemButtonLocation = new(350, 20);
        private Point DeleteItemButtonLocation = new(350, 70);

        private Point AddItemTextBoxLocation = new(460, 30);
        private readonly string AddItemTextBoxName = "AddItemTextBox";

        private Point UserNameTextBoxLabelLocation = new(373, 143);
        private Point UserNameTextBoxLocation = new(440, 140);

        private Point PortNumberTextBoxLabelLocation = new(360, 115);
        private Point PortNumberTextBoxLocation = new(440, 115);
        private readonly string PortNumberTextBoxName = "PortNumberTextBox";

        private Point SaveButtonLocation = new(460, 70);

        private Point SelectedSetTextBoxLabelLocation = new(355, 185);
        private Point SelectedSetTextBoxLocation = new(440, 180);
        private readonly string SelectedSetTextBoxName = "SelectedSetTextBox";

        private Point AppSettingPortNumberTextBoxLabelLocation = new(30, 270);
        private Point AppSettingPortNumberTextBoxLocation = new(180, 270);
        private readonly string AppSettingPortNumberTextBoxName = "AppSettingPortNumberTextBox";

        public Form1()
        {
            InitializeComponent();
            InitializeComponentOverride();
            this.Load += Form1_Load;
            this.FormClosing += Form1_UnLoad;

            CreateUserScopedCustomSettings();
            CreateApplicationScopedCustomConfigSection();

            CreateAddItemButton();
            CreateDeleteItemButton();
            CreateSaveButton();

            CreateAddItemTextBox();

            CreateComboBox();
            CreateListBox();

            CreateUserNameTextBoxLabel();
            CreateUserNameTextBox();

            CreatePortNumberTextBoxLabel();
            CreatePortNumberTextBox();

            CreateSelectedSetTextBoxLabel();
            CreateSelectedSetTextBox();

            CreateAppSettingPortNumberTextBoxLabel();
            CreateAppSettingPortNumberTextBox();
        }



        // this was never called because the event handler was never subscribed
        //   this.Load += Form1_Load;
        // because I didn't use the designer
        private void Form1_Load(object? sender, EventArgs e)
        {
            // CreateApplicationScopedCustomConfigSection();
        }

        private void Form1_UnLoad(object? sender, EventArgs e)
        {
            if (AppScopeCustomConfigSection != null)
            {
                SectionInformation sectionInformation = AppScopeCustomConfigSection.SectionInformation;
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                SaveApplicationScopedCustomConfigSection(config, sectionInformation);
            }
            else
            {
                string msg = "AppScopeCustomConfigSection is NULL!";
                Log.Error(msg);
                throw new Exception(msg);
            }
        }

        private void CreateApplicationScopedCustomConfigSection()
        {
            AppScopeCustomConfigSection = new MyAppScopeConfigSection();
            SectionInformation sectionInformation = AppScopeCustomConfigSection.SectionInformation;
            // Get the current configuration file.
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //MyAppSettingsCustomConfigSection.PortAppConfig = 707; // test write

            // check if configuration section already exists in file. Add it if is missing.
            ConfigurationSection sectionCheck = config.GetSection(AppScopeCustomConfigSection.Name);

            if(sectionCheck == null)
            {
                config.Sections.Add(AppScopeCustomConfigSection.Name, AppScopeCustomConfigSection);
                SaveApplicationScopedCustomConfigSection(config, sectionInformation);
            }
        }

        private void SaveApplicationScopedCustomConfigSection(Configuration config, SectionInformation sectionInformation)
        {
            if (config == null || sectionInformation == null)
            {
                return;
            }

            sectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Full);
        }

        private void CreateUserScopedCustomSettings()
        {
            UserScopedSettings = new MyUserScopeAppSettings();

            //MyCustomSettings.Reset();

            //MyCustomSettings.AddDogName("shaggy");
            if (!CheckIfUserConfigFileExists())
            {
                UserScopedSettings.Save();
            }
        }

        private bool CheckIfUserConfigFileExists()
        {
            ConfigurationUserLevel level = ConfigurationUserLevel.PerUserRoamingAndLocal;
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(level);
            Log.Information("config file (PerUserRoamingAndLocal): {Config}", configuration.FilePath);

            bool retBool = File.Exists(configuration.FilePath);

            level = ConfigurationUserLevel.None;
            configuration = ConfigurationManager.OpenExeConfiguration(level);
            Log.Information("config file (None): {Config}", configuration.FilePath);

            return retBool; 
        }

        private void CreateSaveButton()
        {
            Button button = new()
            {
                Location = SaveButtonLocation,
                Size = new(100, 40),
                Text = "Save"
            };
            //button.Click += (s, e) => { UserScopedSettings.Save(); };
            button.Click += OnSaveSettings;
            Controls.Add(button);
        }

        private void OnSaveSettings(object? sender, EventArgs e)
        {
            if (UserScopedSettings != null)
            {
                UserScopedSettings.Save();
                _ = MessageBox.Show("Settings Saved", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CreateAddItemButton()
        {
            Button addButton = new()
            {
                Location = AddItemButtonLocation,
                Size = new(100, 40),
                Text = "Add"
            };

            addButton.Click += (s, e) => 
            {
                string str = string.Empty;

                Control textBox = Controls[AddItemTextBoxName];                

                if (textBox != null)
                {
                    str = textBox.Text;
                }

                if (str != string.Empty)
                {
                    AddNewItem(str);
                }                
            };

            Controls.Add(addButton);
        }

        private void CreateDeleteItemButton()
        {
            Button deleteButton = new()
            {
                Location = DeleteItemButtonLocation,
                Size = new(100, 40),
                Text = "Delete"
            };

            deleteButton.Click += (s, e) =>
            {
                DeleteSelectedItem();
            };

            Controls.Add(deleteButton);
        }

        private void AddNewItem(string str)
        {
            ClearAddItemTextBox();
            UserScopedSettings?.AddDogName(str);
            RefreshBothBoxes();
        }

        private void DeleteSelectedItem()
        {
            UserScopedSettings?.DeleteDogName();
            RefreshBothBoxes();
        }

        private void RefreshBothBoxes()
        {
            ComboBox comboBox = (ComboBox)Controls[ComboBoxName];
            if (comboBox != null)
            {
                RefreshListControlDataSource(comboBox);
            }

            ListBox listBox = (ListBox)Controls[ListBoxName];
            if (listBox != null)
            {
                RefreshListControlDataSource(listBox);
            }
        }

        private void RefreshListControlDataSource(ListControl listControl)
        {
            // these next 2 statements seem hokey but got the suggestion from
            // DocWho's post in https://stackoverflow.com/questions/14089342/refresh-combobox-items-easiest-way
            listControl.DataBindings.Clear();
            listControl.DataSource = null;

            SetListControlDataSource(listControl);
            //comboBox.Invalidate(true);
            //comboBox.Refresh();            
        }

        private void SetListControlDataSource(ListControl listControl)
        {

            listControl.DataBindings.Add(new Binding("DataSource",
                                                  UserScopedSettings,
                                                  "DogNames",
                                                  false,
                                                  DataSourceUpdateMode.OnPropertyChanged));

            // Note - this does not work if formattingEnabled Binding parameter is the default 'false'
            listControl.DataBindings.Add(new Binding(nameof(listControl.SelectedIndex),
                                                  UserScopedSettings,
                                                  "SelectedDogNameIndex",
                                                  true,
                                                  DataSourceUpdateMode.OnPropertyChanged));
        }

        private void ClearAddItemTextBox()
        {
            TextBox? textBox = (TextBox)Controls[AddItemTextBoxName];

            if (textBox == null)
            {
                return;
            }
            textBox.Text = string.Empty;
        }

        private void CreateComboBox()
        {
            ComboBox comboBox = new()
            {
                Name = ComboBoxName,
                Location = ComboBoxLocation,
                Size = new(150, 21),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            SetListControlDataSource(comboBox);

            Controls.Add(comboBox);
        }

        private void CreateListBox()
        {
            ListBox listBox = new()
            {
                Name = ListBoxName,
                Location = ListBoxLocation,
                Size = new(150, 150),
                SelectionMode = SelectionMode.MultiExtended
            };

            SetListControlDataSource(listBox);

            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

            Controls.Add(listBox);
        }

        private void ListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            TextBox textBox = (TextBox)Controls[SelectedSetTextBoxName];
            if (textBox != null)
            {
                textBox.Text = GetIListValues(((ListBox)sender).SelectedItems);
            }
        }

        private string GetIListValues(IList list)
        {
            StringBuilder stringBuilder = new();

            foreach (object? item in list)
            {
                _ = stringBuilder.Append(item.ToString()).AppendLine();
            }

            return stringBuilder.ToString();
        }

        private void CreateAddItemTextBox()
        {
            TextBox textBox = new()
            {
                Name = AddItemTextBoxName,
                Location = AddItemTextBoxLocation,
                Size = new(150, 40)
            }; 

            Controls.Add(textBox);
        }

        private void CreateUserNameTextBox()
        {
            TextBox textBox = new()
            {
                Name = "UserNameTextBox",
                Location = UserNameTextBoxLocation,
                Size = new(150, 40)
            };
            textBox.DataBindings.Add(new Binding("Text",
                                                 UserScopedSettings,
                                                 "UserName",
                                                 false,
                                                 DataSourceUpdateMode.OnPropertyChanged));
            Controls.Add(textBox);
        }

        private void CreateUserNameTextBoxLabel()
        {
            Label label = new()
            {
                Location = UserNameTextBoxLabelLocation,
                Text = "User Name",
                Size = new(70, 22)
                //BorderStyle = BorderStyle.FixedSingle
            };

            Controls.Add(label);
        }

        private void CreatePortNumberTextBox()
        {
            TextBox textBox = new()
            {
                Name = PortNumberTextBoxName,
                Location = PortNumberTextBoxLocation,
                Size = new(86, 22)
            };

            textBox.DataBindings.Add(new Binding("Text",
                                                 UserScopedSettings,
                                                 "Port",
                                                 true,
                                                 DataSourceUpdateMode.OnPropertyChanged));
            Controls.Add(textBox);
        }

        private void CreatePortNumberTextBoxLabel()
        {
            Label label = new()
            {
                Location = PortNumberTextBoxLabelLocation,
                Text = "Port Number",
                Size = new(78, 22),
                TextAlign = ContentAlignment.MiddleRight
                //BorderStyle = BorderStyle.FixedSingle
            };

            Controls.Add(label);
        }

        private void CreateSelectedSetTextBoxLabel()
        {
            Label label = new()
            {
                Text = "Selected Items",
                Location = SelectedSetTextBoxLabelLocation,
                Size = new(86, 22)
                //BorderStyle = BorderStyle.FixedSingle
            };

            Controls.Add(label);
        }

        private void CreateSelectedSetTextBox()
        {
            TextBox textBox = new()
            {
                Name = SelectedSetTextBoxName,
                Location = SelectedSetTextBoxLocation,
                Multiline = true,
                Size = new(150, 100)
            };

            textBox.DataBindings.Add(new Binding(nameof(Text),
                                                  Controls[ComboBoxName],
                                                  "SelectedItem",
                                                  true,
                                                  DataSourceUpdateMode.OnPropertyChanged));
            Controls.Add(textBox);
        }

        private void CreateAppSettingPortNumberTextBoxLabel()
        {
            Label label = new()
            {
                Text = "App Setting Port Number",
                Location = AppSettingPortNumberTextBoxLabelLocation,
                Size = new(143, 22),
                TextAlign = ContentAlignment.MiddleRight
                //BorderStyle = BorderStyle.FixedSingle
            };

            Controls.Add(label);
        }

        private void CreateAppSettingPortNumberTextBox()
        {
            TextBox textBox = new()
            {
                Name = AppSettingPortNumberTextBoxName,
                Location = AppSettingPortNumberTextBoxLocation,
                Size = new(150, 100),
                ReadOnly = true
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            try
            {
                ConfigurationSection section = config.GetSection(MyAppScopeConfigSection.SkippySectionName);

                textBox.DataBindings.Add(new Binding(nameof(Text),
                                                      section,
                                                      "PortAppConfig"));
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Error(ex.Message);
            }

            
            Controls.Add(textBox);
        }

        private void InitializeComponentOverride()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(650, 300);
            Name = "AppSettingsTestingMainForm";
            Text = "AppSettings Testing MainForm";
            ResumeLayout(false);
        }

    }

}