using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using TestTaskApi.DAL.Entities;

namespace TestTaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Контроллер медицинских записей.")]
    /// <summary>
    /// Контроллер медицинских записей.
    /// </summary>
    public class MedicalRecordsController : ControllerBase
    {
        private readonly TestTaskContext _context;

        public MedicalRecordsController(TestTaskContext context)
        {
            _context = context;
        }

        // GET: api/MedicalRecords
        /// <summary>
        /// Получение списка медицинских записей.
        /// </summary>
        /// <param name="pageNumber">Номер страницы.</param>
        /// <param name="pageSize">Количество записей на одной странице.</param>
        /// <returns>Список медицинских записей.</returns>
        [HttpGet]
        [SwaggerResponse(200, "Список медицинских записей успешно получен.")]
        [SwaggerResponse(404, "Медицинские записи не найдены.")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecords([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalRecords = await _context.MedicalRecords.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var skip = (pageNumber - 1) * pageSize;

            var records = await _context.MedicalRecords
                .Skip(skip)
                .Take(pageSize)
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
                return NotFound($"Список медицинских записей пуст");
            }

            var paginationInfo = new
            {
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return Ok(new { MedicalRecords = records, Pagination = paginationInfo });
        }


        // GET: api/MedicalRecords/5
        /// <summary>
        /// Получение данных конкретной медицинской записи.
        /// </summary>
        /// <param name="id">Идентификатор медицинской записи.</param>
        /// <returns>Данные медицинской записи.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Медицинская запись успешно найдена.")]
        [SwaggerResponse(404, "Медицинская запись не найдена.")]
        public async Task<ActionResult<MedicalRecordDto>> GetMedicalRecord(int id)
        {

            var medicalRecord = await _context.MedicalRecords
                .Select(m => new MedicalRecordDto
                {
                    MedicalRecordId = m.MedicalRecordId,
                    Diagnosis = m.Diagnosis,
                    DoctorName = m.DoctorName,
                    RecordDate = m.RecordDate.ToString("g"),
                    Recomendations = m.Recomendations,
                    PatientId = m.PatientId
                })
                .FirstOrDefaultAsync(m => m.MedicalRecordId == id);

            if (medicalRecord == null)
            {
                return NotFound("Медицинская запись не найдена.");
            }

            return Ok(medicalRecord);
        }

        // POST: api/MedicalRecords
        /// <summary>
        /// Добавление новой медицинской записи.
        /// </summary>
        /// <param name="medicalRecordDto">Данные медицинской записи для создания.</param>
        /// <returns>Данные созданной медицинской записи.</returns>
        [HttpPost]
        [SwaggerResponse(201, "Медицинская запись успешно создана.")]
        [SwaggerResponse(400, "Ошибка валидации или данные некорректны.")]
        public async Task<ActionResult<MedicalRecordDto>> CreateMedicalRecord(MedicalRecordDto medicalRecordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recordDate = DateTime.Parse(medicalRecordDto.RecordDate).ToUniversalTime();

            var medicalRecord = new MedicalRecord
            {
                RecordDate = recordDate,
                Diagnosis = medicalRecordDto.Diagnosis,
                DoctorName = medicalRecordDto.DoctorName,
                Recomendations = medicalRecordDto.Recomendations,
                PatientId = medicalRecordDto.PatientId
            };

            _context.MedicalRecords.Add(medicalRecord);
            try
            {
                await _context.SaveChangesAsync();
                medicalRecordDto.MedicalRecordId = medicalRecord.MedicalRecordId;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresException &&
            postgresException.SqlState == "23503")
                {
                    return BadRequest("Данный пациент не существует.");
                }
            }

            return CreatedAtAction(nameof(GetMedicalRecord), new { id = medicalRecord.MedicalRecordId }, medicalRecordDto);
        }

        // PUT: api/MedicalRecords/5
        /// <summary>
        /// Изменение данных медицинской записи.
        /// </summary>
        /// <param name="id">Идентификатор изменяемой медицинской записи.</param>
        /// <param name="medicalRecordDto">Данные медицинской записи для изменения.</param>
        /// <returns>Данные измененной медицинской записи.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, "Медицинская запись успешно изменена.")]
        [SwaggerResponse(404, "Медицинская запись не найдена.")]
        public async Task<IActionResult> EditMedicalRecord(int id, MedicalRecordDto medicalRecordDto)
        {
            var recordDate = DateTime.Parse(medicalRecordDto.RecordDate).ToUniversalTime();
            var medicalRecord = new MedicalRecord
            {
                MedicalRecordId = id,
                RecordDate = recordDate,
                Diagnosis = medicalRecordDto.Diagnosis,
                DoctorName = medicalRecordDto.DoctorName,
                Recomendations = medicalRecordDto.Recomendations,
                PatientId = medicalRecordDto.PatientId
            };

            _context.Entry(medicalRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresException &&
            postgresException.SqlState == "23503")
                {
                    return BadRequest("Данный пациент не существует.");
                }
            }

            return Ok("Медицинская запись успешно изменена.");
        }

        // DELETE: api/MedicalRecords/5
        /// <summary>
        /// Удаление медицинской записи.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой медицинской записи.</param>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Медицинская запись успешно удалена.")]
        [SwaggerResponse(404, "Медицинская запись не найдена.")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound($"Медицинская запись не найдена");
            }

            _context.MedicalRecords.Remove(medicalRecord);
            await _context.SaveChangesAsync();

            return Ok("Медицинская запись успешно удалена.");
        }

        private bool MedicalRecordExists(int id)
        {
            return _context.MedicalRecords.Any(e => e.MedicalRecordId == id);
        }
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
