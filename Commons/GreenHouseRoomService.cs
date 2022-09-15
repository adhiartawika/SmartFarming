using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace backend.Commons
{
    public interface IGreenHouseRoomService
    {
        BehaviorSubject<Dictionary<int,List<int>>> ghRoom { get; set; }
        Subject<int> submitSubject { get; set; }
        public void AddToRoom(int room, int userId);
        public void LeaveFromRoom(int room, int userId);
        public List<int> GetUserFrom(int room);
        public void SubmitToRoom(int room);
    }
    public class GreenHouseRoomService : IGreenHouseRoomService
    {
        
        public BehaviorSubject<Dictionary<int, List<int>>> ghRoom { get; set; }
        public Subject<int> submitSubject { get; set; }

        public GreenHouseRoomService()
        {
            this.ghRoom = new BehaviorSubject<Dictionary<int, List<int>>>(new Dictionary<int, List<int>>());
            this.submitSubject = new Subject<int>();
        }

        public void AddToRoom(int room, int userId)
        {
            if (!this.ghRoom.Value.ContainsKey(room))
            {
                var val = this.ghRoom.Value[room];
                if (!val.Contains(userId))
                {
                    val.Add(userId);
                    this.ghRoom.Value[room] = val;
                    var temp = this.ghRoom.Value;
                    this.ghRoom.OnNext(temp);
                }
            };
        }

        public void LeaveFromRoom(int room, int userId)
        {
            if (!this.ghRoom.Value.ContainsKey(room))
            {
                var val = this.ghRoom.Value[room];
                if (!val.Contains(userId))
                {
                    val.Remove(userId);
                    this.ghRoom.Value[room] = val;
                    var temp = this.ghRoom.Value;
                    this.ghRoom.OnNext(temp);
                }
            };
        }

        public List<int> GetUserFrom(int room)
        {
            if (!this.ghRoom.Value.ContainsKey(room))
            {
                return this.ghRoom.Value[room];
                
            };
            return new List<int>();
        }

        public void SubmitToRoom(int room)
        {
            this.submitSubject.OnNext(room);
        }
    }
}
