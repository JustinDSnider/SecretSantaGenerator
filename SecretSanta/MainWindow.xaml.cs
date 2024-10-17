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

namespace SecretSanta
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonAddName_Click(object sender, RoutedEventArgs e)
        {
            string enteredName = textboxAddName.Text;
            if (!string.IsNullOrWhiteSpace(enteredName))
            {
                bool nameExists = listParticipants.Items.Cast<string>().Any(name => name.Equals(enteredName, StringComparison.OrdinalIgnoreCase));

                if (!nameExists)
                {
                    listParticipants.Items.Add(enteredName);
                    textboxAddName.Text = "";
                }
                else
                {
                    MessageBox.Show($"The name \"{enteredName}\" already exists in the list.\n All participant names must be unique.", "Duplicate Name Entered", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void buttonRemoveName_Click(object sender, RoutedEventArgs e)
        {
            if (listParticipants.SelectedItem != null)
            {
                listParticipants.Items.Remove(listParticipants.SelectedItem);
            }
        }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            List<string> givers = listParticipants.Items.Cast<string>().ToList();

            if (givers.Count < 3)
            {
                MessageBox.Show($"You only have {givers.Count} people participating. Please add atleast 3 names to the list and try again.");
            }
            else if (string.IsNullOrWhiteSpace(textboxHost.Text))
            {
                MessageBox.Show($"Please input a name into the \"Host\" field.");
            }
            else if (string.IsNullOrWhiteSpace(textboxLimit.Text))
            {
                MessageBox.Show($"Please input a dollar limit.");
            }
            else if (!DatePickerExchangeHasValue())
            {
                MessageBox.Show($"Please select a date for your gift exchange.");
            }
            else if (!DatePickerExchangeIsFuture())
            {
                MessageBox.Show($"{datepickerExchange.SelectedDate.Value.ToString("yyyy-MM-dd")} is invalid. Select a future date.");
            }
            else
            {
                CreateOutputFolder();

                List<string> receivers = new List<string>(givers);

                Shuffle(receivers);

                for (int i = 0; i < receivers.Count; i++)
                {
                    if (givers[i] == receivers[i])
                    {
                        Shuffle(receivers);
                        i = -1;
                    }
                }
                for (int i = 0; i < givers.Count; i++)
                {
                    string outputFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, $"output\\{givers[i]}.txt");
                    using (StreamWriter sw = new StreamWriter(outputFilePath))
                    {
                        sw.WriteLine($"You are giving a gift to {receivers[i]}!");
                        sw.WriteLine($"The purchase limit is {textboxLimit.Text} and the exchange will take place on {datepickerExchange.SelectedDate.Value.ToString("yyyy-MM-dd")}.");
                        sw.WriteLine($"If there are any issues, contact {textboxHost.Text}.");
                    }
                }
                MessageBox.Show($"Generation Complete! Your files can be found in {System.IO.Path.Combine(Environment.CurrentDirectory, "output")}");
            }
        }

        private static void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private static void CreateOutputFolder()
        {
            string outputDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "output");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            else
            {
                foreach (var file in Directory.EnumerateFiles(outputDirectory))
                {
                    File.Delete(file);
                }
            }
        }

        private void TextBoxLimit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void TextBoxHost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.Text, e.Text.Length - 1) && e.Text[0] != '.' && e.Text[0] != ',' && e.Text[0] != '-' && e.Text[0] != '_')
            {
                e.Handled = true;
            }
        }

        private bool DatePickerExchangeIsFuture()
        {
            DateTime selectedDate = datepickerExchange.SelectedDate.Value;

            return selectedDate >= DateTime.Today;
        }

        private bool DatePickerExchangeHasValue()
        {
            return datepickerExchange.SelectedDate.HasValue;
        }
    }
}
