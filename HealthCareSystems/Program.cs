using System;
using System.Collections.Generic;
using System.Linq;

/*
  HealthSystem_AllInOne.cs
  Single-file implementation of a simple healthcare system that:
   - Uses a generic Repository<T> for entity storage
   - Defines Patient and Prescription classes
   - Builds a Dictionary<int, List<Prescription>> grouped by PatientId
   - Accepts user input to seed data (no hard-coded sizes/values required)
   - Provides methods to print patients and prescriptions for a selected patient
*/

namespace HealthSystemAllInOne
{
    // Generic repository for any entity type
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            items.Add(item);
        }

        public List<T> GetAll() => new List<T>(items);

        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var found = items.FirstOrDefault(predicate);
            if (found == null) return false;
            items.Remove(found);
            return true;
        }
    }

    // Patient entity
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name ?? string.Empty;
            Age = age;
            Gender = gender ?? string.Empty;
        }

        public override string ToString() => $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }

    // Prescription entity
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName ?? string.Empty;
            DateIssued = dateIssued;
        }

        public override string ToString() => $"ID: {Id}, Medication: {MedicationName}, Date: {DateIssued:d}";
    }

    // Health system application (single-file orchestrator)
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        // Seed data interactively from user input
        public void SeedData()
        {
            Console.WriteLine("--- Seed Patients ---");
            int patientCount = ReadIntFromConsole("How many patients would you like to add? ", min: 0);

            for (int i = 0; i < patientCount; i++)
            {
                Console.WriteLine($"\nEnter details for patient #{i + 1}:");
                int id = ReadIntFromConsole("  ID: ", min: 0);
                string name = ReadStringFromConsole("  Full name: ");
                int age = ReadIntFromConsole("  Age: ", min: 0, max: 150);
                string gender = ReadStringFromConsole("  Gender: ");

                // Avoid adding duplicate patient IDs
                if (_patientRepo.GetById(p => ((Patient)p).Id == id) != null)
                {
                    Console.WriteLine($"  Warning: Patient with ID {id} already exists — skipping.");
                    continue;
                }

                _patientRepo.Add(new Patient(id, name, age, gender));
            }

            Console.WriteLine("\n--- Seed Prescriptions ---");
            int prescriptionCount = ReadIntFromConsole("How many prescriptions would you like to add? ", min: 0);

            for (int i = 0; i < prescriptionCount; i++)
            {
                Console.WriteLine($"\nEnter details for prescription #{i + 1}:");
                int id = ReadIntFromConsole("  Prescription ID: ", min: 0);
                int patientId = ReadIntFromConsole("  Patient ID (must match an existing patient): ", min: 0);

                // Ensure patient exists before adding prescription
                var patient = _patientRepo.GetById(p => ((Patient)p).Id == patientId);
                if (patient == null)
                {
                    Console.WriteLine($"  Warning: No patient found with ID {patientId}. Prescription will still be recorded but not linked in the map until a matching patient exists.");
                }

                string med = ReadStringFromConsole("  Medication name: ");
                DateTime dateIssued = ReadDateFromConsole("  Date issued (yyyy-mm-dd): ");

                // Avoid adding duplicate prescription IDs
                if (_prescriptionRepo.GetById(p => ((Prescription)p).Id == id) != null)
                {
                    Console.WriteLine($"  Warning: Prescription with ID {id} already exists — skipping.");
                    continue;
                }

                _prescriptionRepo.Add(new Prescription(id, patientId, med, dateIssued));
            }

            Console.WriteLine("\nSeeding complete.\n");
        }

        // Build prescription map grouped by PatientId
        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var pres in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.TryGetValue(pres.PatientId, out var list))
                {
                    list = new List<Prescription>();
                    _prescriptionMap[pres.PatientId] = list;
                }
                list.Add(pres);
            }
        }

        public void PrintAllPatients()
        {
            var patients = _patientRepo.GetAll();
            if (patients.Count == 0)
            {
                Console.WriteLine("No patients available.");
                return;
            }

            Console.WriteLine("--- All Patients ---");
            foreach (var p in patients)
                Console.WriteLine(p);
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list) ? new List<Prescription>(list) : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var prescriptions = GetPrescriptionsByPatientId(patientId);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine($"No prescriptions found for patient ID {patientId}.");
                return;
            }

            Console.WriteLine($"--- Prescriptions for Patient {patientId} ---");
            foreach (var pres in prescriptions)
                Console.WriteLine(pres);
        }

        // Helper input methods
        private static int ReadIntFromConsole(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value >= min && value <= max)
                    return value;
                Console.WriteLine("  Invalid integer — try again.");
            }
        }

        private static string ReadStringFromConsole(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("  Input cannot be empty — try again.");
            }
        }

        private static DateTime ReadDateFromConsole(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (DateTime.TryParse(input, out var dt)) return dt.Date;
                Console.WriteLine("  Invalid date format — try again (e.g. 2025-08-09).");
            }
        }
    }

    // Single entry point that ties everything together
    public static class Program
    {
        public static void Main()
        {
            var app = new HealthSystemApp();

            try
            {
                // 1) Seed data (user-driven)
                app.SeedData();

                // 2) Build the prescription map
                app.BuildPrescriptionMap();

                // 3) Print patients
                app.PrintAllPatients();

                // 4) Let user query prescriptions for a patient
                Console.WriteLine();
                int queryId = HealthSystemApp_ReadPatientId("Enter a Patient ID to view prescriptions, or -1 to exit: ");
                while (queryId != -1)
                {
                    app.PrintPrescriptionsForPatient(queryId);
                    Console.WriteLine();
                    queryId = HealthSystemApp_ReadPatientId("Enter another Patient ID to view prescriptions, or -1 to exit: ");
                }

                Console.WriteLine("Exiting application. Goodbye!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        private static int HealthSystemApp_ReadPatientId(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out var id)) return id;
                Console.WriteLine("  Invalid ID — enter an integer (or -1 to exit).");
            }
        }
    }
}
