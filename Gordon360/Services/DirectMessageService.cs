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
        private IProfileService _profileService;
        private IAccountService _accountService;


        public DirectMessageService()
        {
            var _unitOfWork = new UnitOfWork();
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
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

                var j = new UserViewModel();
                j.user_id = x.user_id;
                j.user_name = _accountService.Get(x.user_id).ADUserName;
                j.user_avatar = null;

                y.user = j;

                return y;
            });


            return messageModel;

        }

        public MessageViewModel GetSingleMessage(string messageID, string roomID)
        {
            var roomIDParam = new SqlParameter("@room_id", roomID);
            var messageIDParam = new SqlParameter("@message_id", messageID);

            var result = RawSqlQuery<MessageViewModel>.query("GET_SINGLE_MESSAGE_BY_ID @room_id, @message_id", roomIDParam, messageIDParam); //run stored procedure

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            var returnModel = new MessageViewModel();
            var user = new UserViewModel();

            foreach( MessageViewModel messageInfo in result)
            {
                returnModel = messageInfo;
                user.user_id = messageInfo.user_id;
                user.user_avatar = messageInfo.image;
                user.user_name = _accountService.Get(messageInfo.user_id).ADUserName;
                returnModel.user = user;
            }

            return returnModel;
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
                    var localRoomIdParam = new SqlParameter("@room_id", x.room_id);
                    var users = RawSqlQuery<UserViewModel>.query("GET_ALL_USERS_BY_ROOM_ID @room_id", localRoomIdParam);

                    var userSelect = users.Select(i =>
                    {
                        UserViewModel j = new UserViewModel();
                        j.user_id = i.user_id;
                        j.user_name = _accountService.Get(i.user_id).ADUserName;
                        j.user_avatar = null;

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


        public CreateGroupViewModel CreateGroup(String name, bool group, string image, List<String> usernames, SendTextViewModel initialMessage, string userId)
        {
            var nameParam = new SqlParameter("@name", name);
            var groupParam = new SqlParameter("@group", group);
            var groupImageParam = new SqlParameter("@roomImage", System.Data.SqlDbType.VarBinary, -1);

            groupImageParam.Value = DBNull.Value;

            var result = RawSqlQuery<CreateGroupViewModel>.query("CREATE_MESSAGE_ROOM @name, @group, @roomImage", nameParam, groupParam, groupImageParam); //run stored procedure
      
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            List<string> idlist = new List<String>(500);

            foreach (string username in usernames)
            {
                idlist.Add(_accountService.GetAccountByUsername(username).GordonID);
            }

            var groupObject = new CreateGroupViewModel();

            foreach(CreateGroupViewModel model in result)
            {
                groupObject = model;
            }

            foreach (string userid in idlist)
            {
                var studentIdParam = new SqlParameter("@user_id", userid);
                var IdRefreshParam2 = new SqlParameter("@_id", groupObject.id);
                var storeRoooms = RawSqlQuery<MessageViewModel>.query("INSERT_USER_ROOMS @user_id, @_id", studentIdParam, IdRefreshParam2);
                UserViewModel userInfo = new UserViewModel();
                userInfo.user_id = userid;
                userInfo.user_name = _accountService.Get(userid).ADUserName;

                groupObject.users.Add(userInfo);
            }

            SendMessage(initialMessage, userId);

            return groupObject;

        }

        //String id, String room_id, String text, String user_id, bool system, bool sent, bool received, bool pending
        public bool SendMessage(SendTextViewModel textInfo, String user_id)
        {
            DateTime createdAt = DateTime.Now;
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == user_id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@_id", textInfo.id);
            var roomIdParam = new SqlParameter("@room_id", textInfo.room_id);
            var textParam = new SqlParameter("@text", textInfo.text);
            var createdAtParam = new SqlParameter("@createdAt", textInfo.createdAt);
            var userIdParam = new SqlParameter("@user_id", user_id);
            var imageParam = new SqlParameter("@image", System.Data.SqlDbType.VarBinary,-1);
            var videoParam = new SqlParameter("@video", System.Data.SqlDbType.VarBinary, -1);
            var audioParam = new SqlParameter("@audio", System.Data.SqlDbType.VarBinary, -1);
            var systemParam = new SqlParameter("@system", textInfo.system);
            var sentParam = new SqlParameter("@sent", textInfo.sent);
            var receivedParam = new SqlParameter("@received", textInfo.received);
            var pendingParam = new SqlParameter("@pending", textInfo.pending);

            imageParam.Value = DBNull.Value;
            videoParam.Value = DBNull.Value;
            audioParam.Value = DBNull.Value;

            var result = RawSqlQuery<SendTextViewModel>.query("INSERT_MESSAGE @_id, @room_id, @text, @createdAt, @user_id, @image, @video, @audio, @system, @sent, @received, @pending",
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
            

            var result = RawSqlQuery<MessageViewModel>.query("INSERT_USER_ROOMS @user_id, @room_id", userIdParam, roomIdParam); //run stored procedure

            bool returnAnswer = true;

            if (result == null)
            {
                returnAnswer = false;
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }



            return returnAnswer;

        }


        public bool StoreUserConnectionIds(String userId, String connectionId)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == userId);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var userIdParam = new SqlParameter("@user_id", userId);
            var connectionIdParam = new SqlParameter("@connection_id", connectionId);


            var result = RawSqlQuery<MessageViewModel>.query("INSERT_USER_CONNECTION_ID @user_id, @connection_id", userIdParam, connectionIdParam); //run stored procedure

            bool returnAnswer = true;

            if (result == null)
            {
                returnAnswer = false;
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }



            return returnAnswer;

        }

        public List<IEnumerable<ConnectionIdViewModel>> GetUserConnectionIds(List<String> userIds)
        {
            var _unitOfWork = new UnitOfWork();
            foreach(string user in userIds) {
                var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == user);
                if (query == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "One of the accounts was not found." };
                }
            }
            var idList = new List<IEnumerable<ConnectionIdViewModel>>(300);

            foreach (string user in userIds)
            {
                var userIdParam = new SqlParameter("@user_id", user);

                var result = RawSqlQuery<ConnectionIdViewModel>.query("GET_ALL_CONNECTION_IDS_BY_ID @user_id", userIdParam); //run stored procedure

                var model = result.Select(x =>
                {
                    ConnectionIdViewModel y = new ConnectionIdViewModel();
                    y.connection_id = x.connection_id;
                    return y;
                });

                idList.Add(model);
            }

            return idList;

        }

        public bool DeleteUserConnectionIds(String connectionId)
        {
            var connectionIdParam = new SqlParameter("@connection_id", connectionId);

            var result = RawSqlQuery<MessageViewModel>.query("DELETE_USER_CONNECTION_ID  @connection_id", connectionIdParam); //run stored procedure

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
