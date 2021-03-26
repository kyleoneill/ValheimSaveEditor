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
    /// Interaction logic for AppearanceEdit.xaml
    /// </summary>
    public partial class AppearanceEdit : Window
    {
        //TODO - skin and hair tone/color
        private static ValheimData.Character character;
        private static int selectedGender;
        private static int selectedBeard;
        private static int selectedHair;

        public AppearanceEdit(ValheimData.Character passedChar)
        {
            InitializeComponent();
            character = passedChar;
            selectedGender = Array.IndexOf(ValheimData.GendersInternal, passedChar.Gender);
            selectedBeard = Array.IndexOf(ValheimData.BeardsInternal, passedChar.Beard);
            selectedHair = Array.IndexOf(ValheimData.HairsInternal, passedChar.Hair);
            Populate();
        }

        private void Populate()
        {
            //gender
            foreach(string gender in ValheimData.GendersUI)
            {
                GenderSelectBox.Items.Add(gender);
            }
            GenderSelectBox.SelectedIndex = selectedGender;
            //beard
            foreach(string beard in ValheimData.BeardsUI)
            {
                BeardSelectBox.Items.Add(beard);
            }
            BeardSelectBox.SelectedIndex = selectedBeard;
            if(character.Gender == 1)
            {
                BeardSelectBox.IsEnabled = false;
            }
            //hair
            foreach(string hair in ValheimData.HairsUI)
            {
                HairSelectBox.Items.Add(hair);
            }
            HairSelectBox.SelectedIndex = selectedHair;
        }

        private void ChangeSelectedGender(object sender, SelectionChangedEventArgs e)
        {
            selectedGender = GenderSelectBox.SelectedIndex;
            var genderInternal = ValheimData.GendersInternal[selectedGender];
            character.Gender = genderInternal;
            if(genderInternal == 1)
            {
                //Female characters cannot have a beard
                character.Beard = ValheimData.BeardsInternal[0];
                selectedBeard = 0;
                BeardSelectBox.SelectedIndex = 0;
                BeardSelectBox.IsEnabled = false;
            }
            else
            {
                BeardSelectBox.IsEnabled = true;
            }

        }

        private void ChangeSelectedBeard(object sender, SelectionChangedEventArgs e)
        {
            selectedBeard = BeardSelectBox.SelectedIndex;
            character.Beard = ValheimData.BeardsInternal[selectedBeard];

        }

        private void ChangeSelectedHair(object sender, SelectionChangedEventArgs e)
        {
            selectedHair = HairSelectBox.SelectedIndex;
            character.Hair = ValheimData.HairsInternal[selectedHair];
        }
    }
}
