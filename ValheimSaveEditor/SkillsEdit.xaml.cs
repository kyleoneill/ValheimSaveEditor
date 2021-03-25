using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ValheimSaveEditor
{
    /// <summary>
    /// Interaction logic for SkillsEdit.xaml
    /// </summary>
    public partial class SkillsEdit : Window
    {
        private static ValheimData.Character character;
        static ValheimData.Character.SkillName selectedSkill = ValheimData.Character.SkillName.None;

        public SkillsEdit(ValheimData.Character passedChar)
        {
            InitializeComponent();
            character = passedChar;
            Populate();
        }

        private void Populate()
        {
            foreach (ValheimData.Character.Skill skill in character.Skills.Values)
            {
                SkillsSelectBox.Items.Add(ValheimData.ConvertSkillEnum(skill.SkillName));
            }
        }

        //Skills
        private void ChangeSelectedSkill(object sender, SelectionChangedEventArgs e)
        {
            selectedSkill = ValheimData.ConvertSkillEnum(SkillsSelectBox.SelectedItem.ToString());
            var skillLevel = character.Skills[selectedSkill];
            SkillLevelTextbox.Text = skillLevel.Level.ToString();
        }
        private void ChangeSkillLevelEventHandle(object sender, RoutedEventArgs args)
        {
            if(selectedSkill != ValheimData.Character.SkillName.None && selectedSkill != ValheimData.Character.SkillName.All)
            {
                bool result = int.TryParse(SkillLevelTextbox.Text, out int i);
                if (result && i > 0 && i <= 100)
                {
                    character.Skills[selectedSkill].Level = i;
                    character.Skills[selectedSkill].Accumulator = 0;
                }
                else
                {
                    SkillLevelTextbox.Text = "";
                    MessageBox.Show("Please enter a number between 1 and 100 for a skill level.", "Error", MessageBoxButton.OK);
                }
            }
            else
            {
                SkillLevelTextbox.Text = "";
            }
        }

        private void ClickMaxSkills(object sender, EventArgs args)
        {
            foreach (KeyValuePair<ValheimData.Character.SkillName, ValheimData.Character.Skill> entry in character.Skills)
            {
                entry.Value.Level = 100;
                entry.Value.Accumulator = 0;
            }
            if(selectedSkill != ValheimData.Character.SkillName.None)
            {
                SkillLevelTextbox.Text = "100";
            }
        }
    }
}
