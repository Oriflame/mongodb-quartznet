using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Quartz.Spi.MongoDbJobStore.Models;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Repositories
{
    internal class CalendarRepository : BaseRepository<Calendar>
    {
        public CalendarRepository(IMongoDatabase database, string instanceName, string collectionName)
            : base(database, instanceName, CalendarId.CalendarType, collectionName)
        {
        }

        public async Task<bool> CalendarExists(string calendarName)
        {
            return
                await Collection.Find(
                    FilterBuilder.Where(calendar => calendar.Id == new CalendarId(calendarName, InstanceName))).AnyAsync();
        }

        public async Task<Calendar> GetCalendar(string calendarName)
        {
            return
                await Collection.Find(calendar => calendar.Id == new CalendarId(calendarName, InstanceName)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetCalendarNames()
        {
            var names = await Collection.Find(FilterBuilder.And(
                    FilterBuilder.Eq(x => x.Id.InstanceName, InstanceName), 
                    FilterBuilder.Eq(x => x.Id.Type, Type)))
                .Project(detail => detail.Id.CalendarName).ToListAsync();
            return names.Distinct().ToList();

// Does not work with CosmosDB :-(            
//            return await Collection.Distinct(calendar => calendar.Id.CalendarName,
//                calendar => calendar.Id.InstanceName == InstanceName && calendar.Id.Type == Type)
//                .ToListAsync();
            
        } 

        public async Task<long> GetCount()
        {
            return await Collection.Find(calendar => calendar.Id.InstanceName == InstanceName && calendar.Id.Type == Type).CountDocumentsAsync();
        }

        public async Task AddCalendar(Calendar calendar)
        {
            await Collection.InsertOneAsync(calendar);
        }

        public async Task<long> UpdateCalendar(Calendar calendar)
        {
            var result = await Collection.ReplaceOneAsync(cal => cal.Id == calendar.Id, calendar);
            return result.MatchedCount;
        }

        public async Task<long> DeleteCalendar(string calendarName)
        {
            var result = 
                await Collection.DeleteOneAsync(calendar => calendar.Id == new CalendarId(calendarName, InstanceName));
            return result.DeletedCount;
        }
    }
}