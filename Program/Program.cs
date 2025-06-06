using System;
using System.Collections.Generic;
using HospitalSystem;

class Program
{
    static void Main()
    {
        var manager = new HospitalManager();

        var drJohn = new Doctor(1, "Smith", "John", new DateTime(1975, 3, 15));
        drJohn.AddSpecialization(new Specialization("Pediatrician"));

        var drEmily = new Doctor(2, "Jones", "Emily", new DateTime(1980, 6, 20));
        drEmily.AddSpecialization(new Specialization("Cardiologist"));

        var drMichael = new Doctor(3, "Williams", "Michael", new DateTime(1978, 11, 5));
        drMichael.AddSpecialization(new Specialization("Pediatrician"));

        var drOlivia = new Doctor(4, "Brown", "Olivia", new DateTime(1985, 1, 25));
        drOlivia.AddSpecialization(new Specialization("Orthopedist"));

        var drJames = new Doctor(5, "Taylor", "James", new DateTime(1970, 9, 10));
        drJames.AddSpecialization(new Specialization("Ophthalmologist"));

        manager.AddDoctor(drJohn);
        manager.AddDoctor(drEmily);
        manager.AddDoctor(drMichael);
        manager.AddDoctor(drOlivia);
        manager.AddDoctor(drJames);

        Console.WriteLine("Initial list of doctors:");
        PrintDoctors(manager.GetAllDoctors());

        Console.WriteLine("\nAdding 'Internal Medicine' specialization to Dr. Emily Jones");
        drEmily.AddSpecialization(new Specialization("Internal Medicine"));

        Console.WriteLine("\nDoctors list after updating Dr. Emily's specializations:");
        PrintDoctors(manager.GetAllDoctors());

        var drWill = new Doctor(6, "Peterson", "Will", new DateTime(1988, 4, 12));
        drWill.AddSpecialization(new Specialization("Pediatrician"));
        manager.AddDoctor(drWill);

        Console.WriteLine("\nAfter adding Dr. Will Peterson:");
        PrintDoctors(manager.GetAllDoctors());

        bool removed = manager.RemoveDoctor(drJohn.Id);
        Console.WriteLine($"\nRemoving Dr. John Smith: {(removed ? "Success" : "Failed")}");

        Console.WriteLine("\nDoctors list after removal:");
        PrintDoctors(manager.GetAllDoctors());

        var patient1 = new Patient(1, "Johnson", "Anna", new DateTime(2010, 7, 4));
        var patient2 = new Patient(2, "Williams", "David", new DateTime(1985, 2, 16));
        var patient3 = new Patient(3, "Miller", "Sophia", new DateTime(1990, 12, 30));

        manager.AddPatient(patient1);
        manager.AddPatient(patient2);
        manager.AddPatient(patient3);

        Console.WriteLine("\nInitial list of patients:");
        PrintPatients(manager.GetAllPatients());

        var patient4 = new Patient(4, "Taylor", "Chris", new DateTime(2000, 5, 21));
        manager.AddPatient(patient4);

        Console.WriteLine("\nAfter adding new patient Chris Taylor:");
        PrintPatients(manager.GetAllPatients());

        bool patientRemoved = manager.RemovePatient(patient2.Id);
        Console.WriteLine($"\nRemoving patient David Williams: {(patientRemoved ? "Success" : "Failed")}");

        Console.WriteLine("\nPatients list after removal:");
        PrintPatients(manager.GetAllPatients());

        var diagnosis1 = new Diagnosis("J01", "Flu", "Hight fever");
        var diagnosis2 = new Diagnosis("I10", "Hypertension", "High blood pressure");
          
        var visit1 = new VisitRecord(patient1, drWill, DateTime.Today.AddDays(-10), diagnosis1, "Prescribed antibiotics");
        var visit2 = new VisitRecord(patient1, drEmily, DateTime.Today.AddDays(-5), diagnosis2, "Recommended lifestyle changes");

        manager.AddVisitRecordToPatient(patient1.Id, visit1);
        manager.AddVisitRecordToPatient(patient1.Id, visit2);

        Console.WriteLine($"\nMedical records for patient {patient1.GetFullName()} (before update):");
        PrintVisitRecords(manager.GetVisitRecordsForPatient(patient1.Id));

        var diagnosis3 = new Diagnosis("R50", "Cold", "Light coughing");
        var visit3 = new VisitRecord(patient1, drMichael, DateTime.Today, diagnosis3, "Advised rest and hydration");
        manager.AddVisitRecordToPatient(patient1.Id, visit3);

        Console.WriteLine($"\nMedical records for patient {patient1.GetFullName()} (after update):");
        PrintVisitRecords(manager.GetVisitRecordsForPatient(patient1.Id));

        var appointment1 = new Appointment(patient1, drWill, DateTime.Today.AddDays(1).AddHours(9));
        var appointment2 = new Appointment(patient4, drEmily, DateTime.Today.AddDays(2).AddHours(10));

        manager.AddAppointment(appointment1);
        manager.AddAppointment(appointment2);
        
        Console.WriteLine("\nSchedule with two appointments:");
        PrintAllAppointments(manager);

        var updatedAppointment1 = new Appointment(patient1, drWill, DateTime.Today.AddDays(1).AddHours(11));
        bool updated = manager.UpdateAppointment(appointment1, updatedAppointment1);
        Console.WriteLine($"\nUpdating appointment for {patient1.GetFullName()} with Dr. Will Peterson: {(updated ? "Success" : "Failed")}");

        Console.WriteLine("\nSchedule after updating appointment:");
        PrintAllAppointments(manager);

        var newAppointment = new Appointment(patient4, drOlivia, DateTime.Today.AddDays(3).AddHours(14));
        manager.AddAppointment(newAppointment);

        Console.WriteLine($"\nAfter scheduling {patient4.GetFullName()} with Dr. Olivia Brown:");
        PrintAllAppointments(manager);

        var searchPatients = manager.SearchPatientsByName("Taylor", "Chris");
        Console.WriteLine("\nSearch patients by name 'Taylor Chris':");
        PrintPatients(searchPatients);

        var searchDoctors = manager.SearchDoctors(specialization: "Pediatrician");
        Console.WriteLine("\nSearch doctors with specialization 'Pediatrician':");
        PrintDoctors(searchDoctors);

        var startRange = DateTime.Today;
        var endRange = DateTime.Today.AddDays(5);
        var drWillSchedule = manager.GetAppointmentsForDoctorInRange(drWill, startRange, endRange);
        Console.WriteLine($"\nDr. Will Peterson's schedule from {startRange.ToShortDateString()} to {endRange.ToShortDateString()}:");
        PrintAppointments(drWillSchedule);

    }

    static void PrintDoctors(IEnumerable<Doctor> doctors)
    {
        int i = 1;
        foreach (var d in doctors)
        {
            Console.WriteLine($"{i}. Dr. {d.GetFullName()} - Specializations: {string.Join(", ", d.Specializations.Select(s => s.Name))}");
            i++;
        }
    }

    static void PrintPatients(IEnumerable<Patient> patients)
    {
        int i = 1;
        foreach (var p in patients)
        {
            Console.WriteLine($"{i}. {p.GetFullName()}, BirthDate: {p.BirthDate.ToShortDateString()}");
            i++;
        }
    }

    static void PrintVisitRecords(IEnumerable<VisitRecord> records)
    {
        foreach (var rec in records)
        {
            Console.WriteLine($"- Date: {rec.VisitDate.ToShortDateString()}, Doctor: Dr. {rec.Doctor.GetFullName()}, Diagnosis: {rec.Diagnosis}, Notes: {rec.Notes}");
        }
    }

    static void PrintAppointments(IEnumerable<Appointment> appointments)
    {
        foreach (var appt in appointments.OrderBy(a => a.AppointmentDate))
        {
            Console.WriteLine($"- {appt.AppointmentDate}: Patient {appt.Patient.GetFullName()} with Dr. {appt.Doctor.GetFullName()}");
        }
    }

    static void PrintAllAppointments(HospitalManager manager)
    {
        var allAppointments = manager.GetAllAppointments();
        Console.WriteLine("Full schedule of all appointments:");
        PrintAppointments(allAppointments);
    }
}
