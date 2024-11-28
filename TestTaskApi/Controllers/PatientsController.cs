using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using TestTaskApi.DAL.Entities;

namespace TestTaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Контроллер пациентов.")]
    /// <summary>
    /// Контроллер пациентов.
    /// </summary>
    public class PatientsController : ControllerBase
    {
        private readonly TestTaskContext _context;

        public PatientsController(TestTaskContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        /// <summary>
        /// Получение списка пациентов.
        /// </summary>
        /// <param name="pageNumber">Номер страницы.</param>
        /// <param name="pageSize">Количество записей на одной странице.</param>
        /// <returns>Список пациентов.</returns>
        [HttpGet]
        [SwaggerResponse(200, "Список пациентов успешно получен.")]
        [SwaggerResponse(404, "Пациенты не найдены.")]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalPatients = await _context.Patients.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalPatients / pageSize);

            var skip = (pageNumber - 1) * pageSize;

            var patients = await _context.Patients
                .Skip(skip)
                .Take(pageSize)
                .Select(p => new PatientDTO
                {
                    PatientId = p.PatientId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    BirthDate = p.BirthDate.ToString("d"),
                    PolisNumber = p.PolisNumber
                })
                .ToListAsync();

            if (!patients.Any())
            {
                return NotFound($"Список пациентов пуст");
            }

            var paginationInfo = new
            {
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalPatients = totalPatients
            };

            return Ok(new { Patients = patients, Pagination = paginationInfo });
        }


        /// <summary>
        /// Получение медицинских записей конкретного пациента.
        /// </summary>
        /// <param name="id">Идентификатор пациента.</param>
        /// <returns>История болезни пациента.</returns>
        [HttpGet("{id}/medicalRecords")]
        [SwaggerResponse(200, "Медицинские записи успешно получены.")]
        [SwaggerResponse(404, "Медицинские записи для пациента не найдены.")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsFromPatientId(int id)
        {
            var records = await _context.MedicalRecords
            .Where(m => m.PatientId == id)
            .Select(m => new MedicalRecordDto
            {
                MedicalRecordId = m.MedicalRecordId,
                Diagnosis = m.Diagnosis,
                DoctorName = m.DoctorName,
                RecordDate = m.RecordDate.ToString("g"),
                Recomendations = m.Recomendations,
                PatientId = m.PatientId
            })
            .ToListAsync();

            if (!records.Any())
            {
                return NotFound($"Медицинские записи для указанного пациента не найдены.");
            }

            return Ok(records);
        }

        // GET: api/Patients/5
        /// <summary>
        /// Получение данных конкретного пациента.
        /// </summary>
        /// <param name="id">Идентификатор пациента.</param>
        /// <returns>Данные пациента.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Пациент успешно найден.")]
        [SwaggerResponse(404, "Пациент не найден.")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .FindAsync(id);
            if (patient == null)
                return NotFound($"Указанный пациент не найден");
            var patientDto = new PatientDTO
            {
                PatientId = patient.PatientId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                BirthDate = patient.BirthDate.ToString("d"),
                PolisNumber = patient.PolisNumber
            };

            return Ok(patientDto);
        }

        // POST: api/Patients
        /// <summary>
        /// Добавление нового пациента.
        /// </summary>
        /// <param name="patientDto">Данные пациента для создания.</param>
        /// <returns>Данные созданного пациента.</returns>
        [HttpPost]
        [SwaggerResponse(201, "Пациент успешно создан.")]
        [SwaggerResponse(400, "Ошибка валидации или данные некорректны.")]
        public async Task<ActionResult<Patient>> CreatePatient([FromBody] PatientDTO patientDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var birthdayDate = DateTime.Parse(patientDto.BirthDate).ToUniversalTime();

                    var patient = new Patient
                    {
                        FirstName = patientDto.FirstName,
                        LastName = patientDto.LastName,
                        BirthDate = birthdayDate,
                        PolisNumber = patientDto.PolisNumber,
                    };
                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();
                    patientDto.PatientId = patient.PatientId;

                    return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patientDto);
                }
                catch (FormatException)
                {
                        return BadRequest("Неверный формат даты");
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is PostgresException postgresException &&
                postgresException.SqlState == "23505")
                    {
                        return BadRequest("Пациент с данным полисом ОМС уже существует.");
                    }
                }

            }
            return BadRequest(ModelState);
        }

        // PUT: api/Patients/5
        /// <summary>
        /// Изменение данных пациента.
        /// </summary>
        /// <param name="id">Идентификатор изменяемого пациента.</param>
        /// <param name="patientDto">Данные пациента для изменения.</param>
        /// <returns>Данные измененного пациента.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, "Пациент успешно изменен.")]
        [SwaggerResponse(404, "Пациент не найден.")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientDTO patientDto)
        {
            var patient = new Patient
            {
                PatientId = id,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                BirthDate = DateTime.Parse(patientDto.BirthDate).ToUniversalTime(),
                PolisNumber = patientDto.PolisNumber
            };

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound("Пациент не найден.");
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresException &&
            postgresException.SqlState == "23503")
                {
                    return BadRequest("Пациент с данным полисом ОМС уже существует.");
                }
            }
            return Ok("Пациент успешно изменен.");
        }

        // DELETE: api/Patients/5
        /// <summary>
        /// Удаление пациента.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого пациента.</param>
        [HttpDelete("{id}")]
        [SwaggerResponse(204, "Пациент успешно удалён.")]
        [SwaggerResponse(404, "Пациент не найден.")]
        [SwaggerResponse(400, "Пациент не может быть удалён из-за связанных записей.")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound("Пациент не найден.");
            }
            try
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresException &&
            postgresException.SqlState == "23503")
                {
                    return BadRequest("Пациент не может быть удалён из-за связанных записей.");
                }
            }
            return Ok("Пациент успешно удалён.");
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
