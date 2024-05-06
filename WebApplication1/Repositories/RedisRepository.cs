using Newtonsoft.Json;
using StackExchange.Redis;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class RedisRepository
    {

        private IDatabase db;
        public RedisRepository(string redisConnection)
        {
            //creating the connectionstring
            var connection = ConnectionMultiplexer.Connect(
               redisConnection);
            db = connection.GetDatabase();

        }

        //public List<Menu> GetMenus()
        //{
        // var listOfMenus = JsonConvert.DeserializeObject<List<Menu>>("menus");
        //
        //}

        ////Optional:
        //public void SetMenus(List<Menu> menus)
        //{
        //  string menusStr = JsonConvert.SerializeObject(menus);

        //}

        public int GetCounter()
        {
            string counter = db.StringGet("counter_msd63a");
            if (string.IsNullOrEmpty(counter) == true)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(counter);
            }
        }

        public void IncrementCounter()
        {
            var counter = GetCounter();
            counter++;
            db.StringSet("counter_msd63a", counter);
        }

    }
}
