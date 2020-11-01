using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;
using System.Diagnostics;


namespace Gordon360.Services
{
    public class DirectMessageService : IDirectMessageService
    {


        public DirectMessageService()
        {
        }

        public IEnumerable<MessageViewModel> GetMessages(string roomId)
        {
        
            var roomIdParam = new SqlParameter("@room_id", roomId);
            var result = RawSqlQuery<MessageViewModel>.query("GET_ALL_MESSAGES_BY_ID @room_id", roomIdParam); //run stored procedure

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }


            var messageModel = result.Select(x =>
            {
                MessageViewModel y = new MessageViewModel();

                y.message_id = x.message_id;
                y.text = x.text;
                y.createdAt = x.createdAt;
                y.user_id = x.user_id;
                y.image = x.image;
                y.video = x.video;
                y.audio = x.audio;
                y.system = x.system;
                y.image = x.image;
                y.received = x.received;
                y.pending = x.pending;
                return y;
            });


            return messageModel;

        }

        public IEnumerable<GroupViewModel> GetRooms(string userId)
        {

            var userIdParam = new SqlParameter("@user_id", userId);

            var result = RawSqlQuery<GroupViewModel>.query("GET_ALL_ROOMS_BY_ID @user_id", userIdParam); //run stored procedure

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }


            var GroupModel = result.Select(x =>
            {
                GroupViewModel y = new GroupViewModel();

                y.room_id = x.room_id;

                return y;
            });


            return GroupModel;

        }

        public List<Object> GetRoomById(string userId)
        {

            var userIdParam = new SqlParameter("@user_id", userId);
            var result = RawSqlQuery<GroupViewModel>.query("GET_ALL_ROOMS_BY_ID @user_id", userIdParam); //run stored procedure

            List<Object> endresult = new List<object>();

            var RoomIdModel = result.Select(x =>
            {
                GroupViewModel y = new GroupViewModel();
                y.room_id = x.room_id;
                return y;
            });

            foreach (GroupViewModel ids in RoomIdModel) {

                var roomIdParam = new SqlParameter("@room_id", ids.room_id);
                var result2 = RawSqlQuery<RoomViewModel>.query("GET_ROOM_BY_ID @room_id", roomIdParam);

                var RoomModel = result2.Select(x =>
                {

                    RoomViewModel y = new RoomViewModel();

                    y.room_id = x.room_id;
                    y.name = x.name;
                    y.group = x.group;
                    y.createdAt = x.createdAt;
                    y.lastUpdated = x.lastUpdated;
                    y.roomImage = x.roomImage;
                    var localRoomIdParam = new SqlParameter("@room_id", 7);
                    var users = RawSqlQuery<UserViewModel>.query("GET_ALL_USERS_BY_ROOM_ID @room_id", localRoomIdParam);

                    var userSelect = users.Select(i =>
                    {
                        UserViewModel j = new UserViewModel();
                        j.user_id = i.user_id;

                        return j;
                    });

                    y.users = userSelect;

                    return y;
                });

                endresult.Add(RoomModel);
            }

            if (endresult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
     


            return endresult;

        }

        public bool CreateGroup(String id, String name, bool group, DateTime lastUpdated)
        {
            DateTime createdAt = DateTime.Now;

            var idParam = new SqlParameter("@_id", id);
            var nameParam = new SqlParameter("@name", name);
            var groupParam = new SqlParameter("@group", group);
            var createdAtParam = new SqlParameter("@createdAt", createdAt);
            var lastUpdatedParam = new SqlParameter("@lastUpdated", lastUpdated);
            var groupImageParam = new SqlParameter("@roomImage", null);

            var result = RawSqlQuery<WellnessViewModel>.query("CREATE_MESSAGE_ROOM @_id, @name, @group, @createdAt, @lastUpdated, @roomImage", idParam, nameParam, groupParam, createdAtParam, lastUpdatedParam, groupImageParam); //run stored procedure
            bool returnAnswer = true; 
            if (result == null)
            {
                returnAnswer = false;
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }



            return returnAnswer;

        }

        public bool SendMessage(String id, String room_id, String text, String user_id, bool system, bool sent, bool received, bool pending)
        {
            DateTime createdAt = DateTime.Now;
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@_id", id);
            var roomIdParam = new SqlParameter("@room_id", room_id);
            var textParam = new SqlParameter("@text", text);
            var createdAtParam = new SqlParameter("@createdAt", createdAt);
            var userIdParam = new SqlParameter("@user_id", user_id);
            var imageParam = new SqlParameter("@image", null);
            var videoParam = new SqlParameter("@video", null);
            var audioParam = new SqlParameter("@audio", null);
            var systemParam = new SqlParameter("@system", system);
            var sentParam = new SqlParameter("@sent", sent);
            var receivedParam = new SqlParameter("@received", received);
            var pendingParam = new SqlParameter("@pending", pending);

            var result = RawSqlQuery<WellnessViewModel>.query("INSERT_MESSAGE @_id, @room_id, @text, @createdAt,@user_id, @image, @video, @audio, @system, @sent, @received, @pending", 
                idParam, roomIdParam, textParam, createdAtParam, userIdParam, imageParam, videoParam, audioParam, systemParam, sentParam, receivedParam, pendingParam); //run stored procedure

            bool returnAnswer = true;
            if (result == null)
            {
                returnAnswer = false;
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }



            return returnAnswer;

        }

        public bool StoreUserRooms(String userId, String roomId)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == userId);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var userIdParam = new SqlParameter("@user_id", userId);
            var roomIdParam = new SqlParameter("@room_id", roomId);
            

            var result = RawSqlQuery<WellnessViewModel>.query("INSERT_USER_ROOMS @user_id, @room_id", userIdParam, roomIdParam); //run stored procedure

            bool returnAnswer = true;

            if (result == null)
            {
                returnAnswer = false;
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }



            return returnAnswer;

        }


    }

}
