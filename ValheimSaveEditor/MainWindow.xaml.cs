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
        static ValheimData.Character.SkillName selectedSkill;

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
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "FCH Files (*.fch)|*.fch";
            dlg.InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Appdata\LocalLow\IronGate\Valheim\characters");
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
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "FCH Files (*.fch)|*.fch";
            dlg.InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Appdata\LocalLow\IronGate\Valheim\characters");
            dlg.FileName = character.Name;
            dlg.DefaultExt = ".fch";
            Nullable<bool> result = dlg.ShowDialog();
            if(result == true)
            {
                string fileName = dlg.FileName;
                byte[] characterAsBytes = FchParser.CharacterToArray(character);
                File.WriteAllBytes(fileName, characterAsBytes);
            }
        }

        private void MenuItem_ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Populate the form when a character is loaded
        private void PopulateForm()
        {
            //name
            CharNameTextbox.Text = character.Name;

            //skills
            foreach(ValheimData.Character.Skill skill in character.Skills.Values)
            {
                SkillsSelectBox.Items.Add(ValheimData.ConvertSkillEnum(skill.SkillName));
            }
        }

        //Change character name
        private void ChangeNameEventHandle(object sender, TextChangedEventArgs args)
        {
            if(isCharacterLoaded)
            {
                character.Name = CharNameTextbox.Text;
            }
        }

        //Skills
        private void ChangeSelectedSkill(object sender, SelectionChangedEventArgs e)
        {
            if (isCharacterLoaded)
            {
                selectedSkill = ValheimData.ConvertSkillEnum(SkillsSelectBox.SelectedItem.ToString());
                var skillLevel = character.Skills[selectedSkill];
                SkillLevelTextbox.Text = skillLevel.Level.ToString();
            }
        }
        private void ChangeSkillLevelEventHandle(object sender, TextChangedEventArgs args)
        {
            if(isCharacterLoaded)
            {
                bool result = int.TryParse(SkillLevelTextbox.Text, out int i);
                if (result && i > 0 && i < 100)
                {
                    character.Skills[selectedSkill].Level = i;
                }
            }
        }
        private void ClickMaxSkills(object sender, EventArgs args)
        {
            if(isCharacterLoaded)
            {
                foreach(KeyValuePair<ValheimData.Character.SkillName, ValheimData.Character.Skill> entry in character.Skills)
                {
                    entry.Value.Level = 100;
                    entry.Value.Accumulator = 0;
                }
                SkillLevelTextbox.Text = "100";
            }
        }
    }
}
