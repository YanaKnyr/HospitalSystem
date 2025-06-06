using System;
using System.Collections.Generic;

namespace HospitalSystem
{
    public abstract class Person
    {
        public int Id { get; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime BirthDate { get; set; }

        public Person(int id, string lastName, string firstName, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("Name and surname cannot be empty.");
            }

            Id = id;
            LastName = lastName;
            FirstName = firstName;
            BirthDate = birthDate;
        }
        public string GetFullName() => $"{LastName} {FirstName}";
    }

    public class Patient : Person
    {
        public MedicalCard MedicalCard { get; }

        public Patient(int id, string lastName, string firstName, DateTime birthDate)
            : base(id, lastName, firstName, birthDate)
        {
            MedicalCard = new MedicalCard(); 
        }

        public void AddMedicalRecord(VisitRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            MedicalCard.AddVisit(record);
        }
    }

    public class Doctor : Person
    {
        private readonly List<Specialization> _specializations = new();

        public IReadOnlyList<Specialization> Specializations => _specializations.AsReadOnly();

        public Doctor(int id, string lastName, string firstName, DateTime birthDate): base(id, lastName, firstName, birthDate ) { }

        public void UpdateSpecializations(IEnumerable<Specialization> newSpecializations)
        {
            if (newSpecializations == null)
            {
                throw new ArgumentNullException(nameof(newSpecializations));
            }

            var distinctSpecs = newSpecializations.Distinct().ToList();

            if (distinctSpecs.Count > 10)
            {
                throw new InvalidOperationException("A doctor can have maximum 10 specializations.");
            }

            _specializations.Clear();
            _specializations.AddRange(distinctSpecs);
        }


        public void AddSpecialization(Specialization specialization)
        {
            if (specialization == null)
            {
                throw new ArgumentNullException(nameof(specialization));
            }

            if (_specializations.Contains(specialization))
            {
                return;
            }

            if (_specializations.Count >= 10)
            {
                throw new InvalidOperationException("A doctor can have at most 10 specializations.");
            }

            _specializations.Add(specialization);
        }

        public bool RemoveSpecialization(Specialization specialization)
        {
            return _specializations.Remove(specialization);
        }
    }

    public class VisitRecord
    {
        public Patient Patient { get; }

        public Doctor Doctor { get; }

        public DateTime VisitDate { get; }

        public Diagnosis Diagnosis { get; }

        public string? Notes { get; }

        public VisitRecord(Patient patient, Doctor doctor, DateTime visitDate, Diagnosis diagnosis, string? notes = null)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }

            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor));
            }

            if (diagnosis == null)
            {
                throw new ArgumentNullException(nameof(diagnosis));
            }

            Patient = patient;
            Doctor = doctor;
            VisitDate = visitDate;
            Diagnosis = diagnosis;
            Notes = notes;
        }
    }

    public class Diagnosis
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public Diagnosis(string code, string name, string? description = null)
        { 
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Code and name of diagnosis cannot be empty.");
            }

            Code = code;
            Name = name;
            Description = description;
        }

        public override string ToString() => $"{Code}: {Name}";
    }

    public class MedicalCard
    {
        private const int MaxRecords = 100;
        private readonly List<VisitRecord> _records = new();

        public IReadOnlyList<VisitRecord> VisitRecords => _records.AsReadOnly();

        public void AddVisit(VisitRecord record)
        {
            if (_records.Count >= MaxRecords)
            {
                throw new InvalidOperationException("The medical card can onle have 100 records.");
            }
            _records.Add(record);
        }

        public bool RemoveVisit(VisitRecord record)
        {
            return _records.Remove(record);
        }
    }

    public class Appointment
    {
        public Patient Patient { get; }

        public Doctor Doctor { get; }

        public DateTime AppointmentDate { get; }

        public Appointment(Patient patient, Doctor doctor, DateTime appointmentDate)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
            }

            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor), "Doctor cannot be null.");
            }

            Patient = patient;
            Doctor = doctor;
            AppointmentDate = appointmentDate;
        }
    }

    public class Schedule
    {
        private readonly List<Appointment> _appointments = new();

        public IReadOnlyList<Appointment> Appointments => _appointments.AsReadOnly();

        public void AddAppointment(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            }
            _appointments.Add(appointment);
        }

        public void RemoveAppointment(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            }

            bool removed = _appointments.Remove(appointment);
            if (!removed)
            {
                throw new InvalidOperationException("Appointment not found in the schedule.");
            }
        }

        public IEnumerable<Appointment> GetAppointmentsForDoctor(Doctor doctor)
        {
            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor), "Doctor cannot be null.");
            }

            var result = new List<Appointment>();

            foreach (var appointment in _appointments)
            {
                if (appointment.Doctor == doctor)
                {
                    result.Add(appointment);
                }
            }

            return result;
        }

        public IEnumerable<Appointment> GetAppointmentsForPatient(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
            }

            var result = new List<Appointment>();

            foreach (var appointment in _appointments)
            {
                if (appointment.Patient == patient)
                {
                    result.Add(appointment);
                }
            }

            return result;
        }
    }

    public class Specialization
    {
        public string Name { get; }

        public Specialization(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            }
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Specialization other && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.ToLowerInvariant().GetHashCode();
        }

        public override string ToString() => Name;
    }

    public class HospitalManager
    {
        private readonly List<Doctor> _doctors = new();
        private readonly List<Patient> _patients = new();
        private readonly Schedule _schedule = new();

        public void AddDoctor(Doctor doctor)
        {
            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor));
            }
            _doctors.Add(doctor);
        }

        public bool RemoveDoctor(int doctorId)
        {
            var doctor = _doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
            {
                throw new InvalidOperationException($"Doctor with ID {doctorId} can not be removed");
            }
            return _doctors.Remove(doctor);
        }

        public bool UpdateDoctor(int doctorId, string newLastName, string newFirstName, DateTime newBirthDate, IEnumerable<Specialization>? newSpecializations = null)
        {
            var doctor = _doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor == null)
            {
                throw new InvalidOperationException($"Doctor with ID {doctorId} can not be updated.");
            }

            doctor.LastName = newLastName;
            doctor.FirstName = newFirstName;
            doctor.BirthDate = newBirthDate;

            if (newSpecializations != null)
            {
                doctor.UpdateSpecializations(newSpecializations);
            }

            return true;
        }

        public List<Doctor> GetAllDoctors()
        {
            return new(_doctors);
        }

        public void AddPatient(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            _patients.Add(patient);
        }

        public bool RemovePatient(int patientId)
        {
            var patient = _patients.FirstOrDefault(p => p.Id == patientId);
            if (patient == null)
            {
                throw new InvalidOperationException($"Patient with Id {patientId} can not be removed.");
            }
            return _patients.Remove(patient);
        }

        public Patient? GetPatientById(int id)
        {
            return _patients.FirstOrDefault(p => p.Id == id);
        }

        public List<Patient> SearchPatientsByName(string lastName, string firstName)
        {
            var results = new List<Patient>();

            foreach (var p in _patients)
            {
                if (p.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase) &&
                    p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(p);
                }
            }

            return results;
        }

        public List<Patient> GetAllPatients()
        {
            return new(_patients);
        }

        public void AddAppointment(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            var time = appointment.AppointmentDate.TimeOfDay;

            if (time < TimeSpan.FromHours(8) || time > TimeSpan.FromHours(19))
            {
                throw new InvalidOperationException("Appointments can only be scheduled between 8:00 and 19:00.");
            }

            bool conflict = _schedule.GetAppointmentsForDoctor(appointment.Doctor)
                .Any(a => a.AppointmentDate == appointment.AppointmentDate);

            if (conflict)
            {
                throw new InvalidOperationException("The doctor already has an appointment at this time.");
            }

            _schedule.AddAppointment(appointment);
        }

        public void RemoveAppointment(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }
            if (!_schedule.Appointments.Contains(appointment))
            {
                throw new InvalidOperationException("Appointment was not found in base.");
            }
            _schedule.RemoveAppointment(appointment);
        }

        public bool UpdateAppointment(Appointment oldAppointment, Appointment newAppointment)
        {
            if (oldAppointment == null || newAppointment == null)
            {
                throw new ArgumentNullException("Appointment can not be null.");
            }

            var time = newAppointment.AppointmentDate.TimeOfDay;

            if (time < TimeSpan.FromHours(8) || time > TimeSpan.FromHours(19))
            {
                throw new InvalidOperationException("Appointments must be between 8:00 and 19:00.");
            }

            bool conflict = _schedule.GetAppointmentsForDoctor(newAppointment.Doctor)
                .Any(a => a.AppointmentDate == newAppointment.AppointmentDate && a != oldAppointment);

            if (conflict)
            {
                throw new InvalidOperationException("The doctor already has an appointment at this time.");
            }

            _schedule.RemoveAppointment(oldAppointment);
            _schedule.AddAppointment(newAppointment);
            return true;
        }

        public IReadOnlyList<Appointment> GetAllAppointments()
        {
            return _schedule.Appointments.OrderBy(a => a.AppointmentDate).ToList().AsReadOnly();
        }

        public void AddVisitRecordToPatient(int patientId, VisitRecord record)
        {
            var patient = GetPatientById(patientId);
            if (patient == null)
            {
                throw new ArgumentException("Patient was not found");
            }
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            patient.MedicalCard.AddVisit(record);
        }

        public IReadOnlyList<VisitRecord> GetVisitRecordsForPatient(int patientId)
        {
            var patient = GetPatientById(patientId);
            if (patient == null)
            {
                throw new ArgumentException("Patient was not found");
            }
            return patient.MedicalCard.VisitRecords;
        }

        public bool RemoveVisitRecordFromPatient(int patientId, VisitRecord record)
        {
            var patient = GetPatientById(patientId);
            if (patient == null || record == null)
            {
                return false;
            }
            return patient.MedicalCard.RemoveVisit(record);
        }

        public List<Doctor> SearchDoctors(string? lastName = null, string? firstName = null, string? specialization = null)
        {
            var results = new List<Doctor>();

            foreach (var d in _doctors)
            {
                bool matchesLastName = string.IsNullOrWhiteSpace(lastName) || d.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase);
                bool matchesFirstName = string.IsNullOrWhiteSpace(firstName) || d.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase);

                bool matchesSpecialization = true;
                if (!string.IsNullOrWhiteSpace(specialization))
                {
                    matchesSpecialization = false;
                    foreach (var s in d.Specializations)
                    {
                        if (s.Name.Equals(specialization, StringComparison.OrdinalIgnoreCase))
                        {
                            matchesSpecialization = true;
                            break;
                        }
                    }
                }

                if (matchesLastName && matchesFirstName && matchesSpecialization)
                {
                    results.Add(d);
                }
            }

            return results;
        }

        public IEnumerable<Appointment> GetAppointmentsForDoctor(Doctor doctor)
        {
            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor));
            }
            return _schedule.GetAppointmentsForDoctor(doctor);
        }

        public IEnumerable<Appointment> GetAppointmentsForPatient(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            return _schedule.GetAppointmentsForPatient(patient);
        }

        public IEnumerable<Appointment> GetAppointmentsForDoctorInRange(Doctor doctor, DateTime startDate, DateTime endDate)
        {
            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor));
            }

            var allAppointments = _schedule.GetAppointmentsForDoctor(doctor);
            var results = new List<Appointment>();

            foreach (var appointment in allAppointments)
            {
                if (appointment.AppointmentDate >= startDate && appointment.AppointmentDate <= endDate)
                {
                    results.Add(appointment);
                }
            }

            return results;
        }

        public IEnumerable<Appointment> GetAppointmentsAtTime(DateTime time)
        {
            var results = new List<Appointment>();

            foreach (var appointment in _schedule.Appointments)
            {
                if (appointment.AppointmentDate == time)
                {
                    results.Add(appointment);
                }
            }

            return results;
        }
    }

}
