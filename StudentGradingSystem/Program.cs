using System;
using System.Collections.Generic;

public class Repository<T>
{
    private readonly List<T> items = new List<T>();

    public void Add(T item) => items.Add(item);

    public void Remove(Predicate<T> match) => items.RemoveAll(match);

    public List<T> GetAll() => new List<T>(items);

    public T Find(Predicate<T> match) => items.Find(match);

    public void Update(Predicate<T> match, Action<T> updateAction)
    {
        T item = items.Find(match);
        if (item != null)
        {
            updateAction(item);
        }
    }
}

public class Patient
{
    public int PatientId { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

public class Prescription
{
    public int PrescriptionId { get; set; }
    public int PatientId { get; set; }
    public string MedicationName { get; set; }
    public int DosageMg { get; set; }
}

public class Program
{
    private static Repository<Patient> patientRepo = new Repository<Patient>();
    private static Repository<Prescription> prescriptionRepo = new Repository<Prescription>();
    private static Dictionary<int, List<Prescription>> prescriptionMap = new Dictionary<int, List<Prescription>>();

    public static void Main()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n--- Healthcare System Menu ---");
            Console.WriteLine("1. Add Patient");
            Console.WriteLine("2. Add Prescription");
            Console.WriteLine("3. View All Patients");
            Console.WriteLine("4. View Prescriptions for Patient");
            Console.WriteLine("5. Update Patient");
            Console.WriteLine("6. Delete Patient");
            Console.WriteLine("7. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddPatient();
                    break;
                case "2":
                    AddPrescription();
                    break;
                case "3":
                    ViewAllPatients();
                    break;
                case "4":
                    ViewPrescriptionsForPatient();
                    break;
                case "5":
                    UpdatePatient();
                    break;
                case "6":
                    DeletePatient();
                    break;
                case "7":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private static void AddPatient()
    {
        Console.Write("Enter Patient ID: ");
        int id = int.Parse(Console.ReadLine());
        Console.Write("Enter Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Age: ");
        int age = int.Parse(Console.ReadLine());

        patientRepo.Add(new Patient { PatientId = id, Name = name, Age = age });
        Console.WriteLine("Patient added successfully.");
    }

    private static void AddPrescription()
    {
        Console.Write("Enter Prescription ID: ");
        int pid = int.Parse(Console.ReadLine());
        Console.Write("Enter Patient ID: ");
        int patientId = int.Parse(Console.ReadLine());
        Console.Write("Enter Medication Name: ");
        string medName = Console.ReadLine();
        Console.Write("Enter Dosage (mg): ");
        int dosage = int.Parse(Console.ReadLine());

        var prescription = new Prescription
        {
            PrescriptionId = pid,
            PatientId = patientId,
            MedicationName = medName,
            DosageMg = dosage
        };

        prescriptionRepo.Add(prescription);

        if (!prescriptionMap.ContainsKey(patientId))
            prescriptionMap[patientId] = new List<Prescription>();

        prescriptionMap[patientId].Add(prescription);
        Console.WriteLine("Prescription added successfully.");
    }

    private static void ViewAllPatients()
    {
        var patients = patientRepo.GetAll();
        if (patients.Count == 0)
        {
            Console.WriteLine("No patients found.");
            return;
        }

        Console.WriteLine("\n--- All Patients ---");
        foreach (var p in patients)
        {
            Console.WriteLine($"ID: {p.PatientId}, Name: {p.Name}, Age: {p.Age}");
        }
    }

    private static void ViewPrescriptionsForPatient()
    {
        Console.Write("Enter Patient ID: ");
        int id = int.Parse(Console.ReadLine());

        if (prescriptionMap.ContainsKey(id))
        {
            Console.WriteLine($"Prescriptions for Patient {id}:");
            foreach (var pres in prescriptionMap[id])
            {
                Console.WriteLine($"RxID: {pres.PrescriptionId}, Medication: {pres.MedicationName}, Dosage: {pres.DosageMg}mg");
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }

    private static void UpdatePatient()
    {
        Console.Write("Enter Patient ID to update: ");
        int id = int.Parse(Console.ReadLine());

        patientRepo.Update(p => p.PatientId == id, p =>
        {
            Console.Write("Enter New Name: ");
            p.Name = Console.ReadLine();
            Console.Write("Enter New Age: ");
            p.Age = int.Parse(Console.ReadLine());
        });

        Console.WriteLine("Patient updated successfully.");
    }

    private static void DeletePatient()
    {
        Console.Write("Enter Patient ID to delete: ");
        int id = int.Parse(Console.ReadLine());

        patientRepo.Remove(p => p.PatientId == id);
        prescriptionMap.Remove(id);
        Console.WriteLine("Patient and related prescriptions deleted.");
    }
}
