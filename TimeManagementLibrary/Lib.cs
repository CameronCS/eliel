namespace TimeManagementLibrary {
    public class Module {
        public string Code {
            get; set;
        }
        public string Name {
            get; set;
        }
        public int Credits {
            get; set;
        }
        public int ClassHoursPerWeek {
            get; set;
        }

        public int SelfStudyHoursPerWeek {
            get; set;
        }

        public int RemainingHours {
            get; set;
        }
    }

    // Semester.cs
    public class Semester {
        public List<Module> Modules {
            get; set;
        }
        public int NumberOfWeeks {
            get; set;
        }
        public DateTime StartDate {
            get; set;
        }

        public Semester() {
            Modules = new List<Module>();
        }
    }

    // ModuleManager.cs
    public class ModuleManager {
        public static int CalculateSelfStudyHours(Module module, Semester semester) {
            int selfStudyHoursPerWeek = (module.Credits * 10) / (semester.NumberOfWeeks - module.ClassHoursPerWeek);
            return selfStudyHoursPerWeek;
        }
    }
}