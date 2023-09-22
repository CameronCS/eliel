using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TimeManagementLibrary;

namespace WPF {
    public partial class MainWindow : Window {
        private ObservableCollection<Module> modules;
        private ObservableCollection<Module> remainingHours;
        private Semester semester;
        private Dictionary<Module, Dictionary<DateTime, int>> recordedHoursDictionary;

        public MainWindow() {
            InitializeComponent();
            InitializeData();
            InitializeUI();
        }

        private void InitializeData() {
            modules = new ObservableCollection<Module>();
            remainingHours = new ObservableCollection<Module>();
            semester = new Semester();
            recordedHoursDictionary = new Dictionary<Module, Dictionary<DateTime, int>>();
        }

        private void InitializeUI() {
            moduleListView.ItemsSource = modules;
            remainingHoursListView.ItemsSource = remainingHours;
        }

        private void AddModule_Click(object sender, RoutedEventArgs e) {
            // Get values from textboxes and create a new module
            string code = moduleCodeTextBox.Text;
            string name = moduleNameTextBox.Text;
            int credits = int.Parse(moduleCreditsTextBox.Text);
            int classHours = int.Parse(moduleClassHoursTextBox.Text);

            Module module = new Module { Code = code, Name = name, Credits = credits, ClassHoursPerWeek = classHours };
            modules.Add(module);

            // Calculate and display self-study hours
            int selfStudyHours = ModuleManager.CalculateSelfStudyHours(module, semester);
            module.SelfStudyHoursPerWeek = selfStudyHours;

            // Clear input fields
            moduleCodeTextBox.Clear();
            moduleNameTextBox.Clear();
            moduleCreditsTextBox.Clear();
            moduleClassHoursTextBox.Clear();

            // Add the module to the ComboBox items source
            moduleComboBox.Items.Add(module);

            // Initialize the recorded hours dictionary entry for the new module
            recordedHoursDictionary.Add(module, new Dictionary<DateTime, int>());
        }

        private void RecordHours_Click(object sender, RoutedEventArgs e) {
            // Get selected module, date, and recorded hours
            Module selectedModule = moduleComboBox.SelectedItem as Module;
            DateTime selectedDate = datePicker.SelectedDate ?? DateTime.MinValue;

            if (selectedModule == null || selectedDate == DateTime.MinValue) {
                MessageBox.Show("Please select a module and date.");
                return;
            }

            if (string.IsNullOrEmpty(hoursTextBox.Text) || !int.TryParse(hoursTextBox.Text, out int recordedHours)) {
                MessageBox.Show("Please enter a valid number of hours.");
                return;
            }

            // Store the recorded hours in the dictionary
            if (!recordedHoursDictionary.ContainsKey(selectedModule)) {
                recordedHoursDictionary[selectedModule] = new Dictionary<DateTime, int>();
            }

            if (!recordedHoursDictionary[selectedModule].ContainsKey(selectedDate)) {
                recordedHoursDictionary[selectedModule][selectedDate] = 0;
            }

            recordedHoursDictionary[selectedModule][selectedDate] += recordedHours;

            // Calculate remaining hours and update the UI
            int remainingHoursForModule = CalculateRemainingHours(selectedModule);
            selectedModule.RemainingHours = remainingHoursForModule;
        }

        private int CalculateRemainingHours(Module module) {
            if (recordedHoursDictionary.ContainsKey(module)) {
                int selfStudyHoursPerWeek = ModuleManager.CalculateSelfStudyHours(module, semester);
                int recordedHoursForWeek = recordedHoursDictionary[module]
                    .Where(kvp => kvp.Key >= DateTime.Now.StartOfWeek() && kvp.Key <= DateTime.Now.EndOfWeek())
                    .Sum(kvp => kvp.Value);

                // Calculate and return remaining hours
                return selfStudyHoursPerWeek - recordedHoursForWeek;
            }

            return 0; // Default to 0 if there are no recorded hours
        }
    }
}

public static class DateTimeExtensions {
    public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday) {
        int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    public static DateTime EndOfWeek(this DateTime date, DayOfWeek endOfWeek = DayOfWeek.Sunday) {
        int diff = (7 + (endOfWeek - date.DayOfWeek)) % 7;
        return date.AddDays(diff).Date;
    }
}
