using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;

namespace ValheimSaveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ValheimData.Character character;
        private static bool isCharacterLoaded;
        private string CharName
        {
            get { return character.Name; }
            set
            {
                if (value.Length < 4 || value.Length > 15)
                {
                    CharErrorTextblock.Text = "Character names have a minimum length of 4 and maximum length of 15 characters.";
                }
                else
                {
                    CharErrorTextblock.Text = "";
                    character.Name = value;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            if (Utility.IsGameRunning())
            {
                MessageBox.Show("Close Valheim before using the save editor", "Warning", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
        }

        private void MenuItem_OpenClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "FCH Files (*.fch)|*.fch",
                InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Appdata\LocalLow\IronGate\Valheim\characters")
            };
            Nullable<bool> result = dlg.ShowDialog();
            if(result == true)
            {
                string filename = dlg.FileName;
                character = FchParser.ReadCharacterData(filename);
                isCharacterLoaded = true;
                PopulateForm();
            }
        }

        private void MenuItem_SaveClick(object sender, RoutedEventArgs e)
        {
            string valheimSaveFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Appdata\LocalLow\IronGate\Valheim\characters");
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "FCH Files (*.fch)|*.fch",
                InitialDirectory = valheimSaveFolder,
                FileName = character.Name,
                DefaultExt = ".fch"
            };
            Nullable<bool> result = dlg.ShowDialog();
            if(result == true)
            {
                if(File.Exists(dlg.FileName))
                {
                    string backupFileName = character.Name + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".fch.backup";
                    File.Move(dlg.FileName, System.IO.Path.Combine(valheimSaveFolder, backupFileName));
                }
                string fileName = dlg.FileName;
                byte[] characterAsBytes = FchParser.CharacterToArray(character);
                File.WriteAllBytes(fileName, characterAsBytes);
            }
        }

        private void MenuItem_ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PopulateForm()
        {
            CharNameTextbox.Text = character.Name;
        }

        private void ChangeNameEventHandle(object sender, TextChangedEventArgs args)
        {
            if(isCharacterLoaded)
            {
                CharName = CharNameTextbox.Text;
            }
        }

        private void HandleEditSkillsButton(object sender, RoutedEventArgs e)
        {
            if (isCharacterLoaded)
            {
                SkillsEdit skillsEdit = new SkillsEdit(character);
                skillsEdit.Show();
            }
        }

        private void HandleEditAppearanceButton(object sender, RoutedEventArgs e)
        {
            if(isCharacterLoaded)
            {
                AppearanceEdit appearanceEdit = new AppearanceEdit(character);
                appearanceEdit.Show();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
