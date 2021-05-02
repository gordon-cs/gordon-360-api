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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

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

        //returns all the messages associated with a certain room id in the form of a list of MessageViewModels
        public IEnumerable<MessageViewModel> GetMessages(string roomId)
        {

            var roomIdParam = new SqlParameter("@room_id", roomId);
            var result = RawSqlQuery<ReturnMessageViewModel>.query("GET_ALL_MESSAGES_BY_ID @room_id", roomIdParam); //run stored procedure

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
                string encodedByteArray = null;
                if (x.image != null) {
                    encodedByteArray = Convert.ToBase64String(x.image);
                }
                y.image = encodedByteArray;
                y.video = x.video;
                y.audio = x.audio;
                y.system = x.system;
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

        //Gets a single message specified by a messageID and RoomID
        public MessageViewModel GetSingleMessage(string messageID, string roomID)
        {
            var roomIDParam = new SqlParameter("@room_id", roomID);
            var messageIDParam = new SqlParameter("@message_id", messageID);

            var result = RawSqlQuery<ReturnMessageViewModel>.query("GET_SINGLE_MESSAGE_BY_ID @room_id, @message_id", roomIDParam, messageIDParam); //run stored procedure

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            var returnModel = new MessageViewModel();
            var user = new UserViewModel();

            foreach( ReturnMessageViewModel messageInfo in result)
            {
                returnModel.message_id = messageInfo.message_id;
                returnModel.text = messageInfo.text;
                returnModel.createdAt = messageInfo.createdAt;
                returnModel.user_id = messageInfo.user_id;
                if(messageInfo.image != null)
                {
                   string encodedByteArray = Convert.ToBase64String(messageInfo.image);
                    returnModel.image = encodedByteArray;
                }
                returnModel.video = messageInfo.video;
                returnModel.audio = messageInfo.audio;
                returnModel.system = messageInfo.system;
                returnModel.received = messageInfo.received;
                returnModel.pending = messageInfo.pending;

                user.user_id = messageInfo.user_id;
                user.user_name = _accountService.Get(messageInfo.user_id).ADUserName;
                returnModel.user = user;
            }

            return returnModel;
        }

        //returns all the room IDs associated with a user id
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
        // get all the room objects associated with a user ID in the form of a list of objects
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
                var result2 = RawSqlQuery<ReturnRoomViewModel>.query("GET_ROOM_BY_ID @room_id", roomIdParam);

                var RoomModel = result2.Select(x =>
                {

                    RoomViewModel y = new RoomViewModel();

                    y.room_id = x.room_id;
                    y.name = x.name;
                    y.group = x.group;
                    y.createdAt = x.createdAt;
                    y.lastUpdated = x.lastUpdated;
                    if (x.roomImage != null)
                    {
                        string encodedByteArray = Convert.ToBase64String(x.roomImage);
                        y.roomImage = encodedByteArray;
                    }
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

        //create group using user information taken from the front end.
        public CreateGroupViewModel CreateGroup(String name, bool group, string image, List<String> usernames, SendTextViewModel initialMessage, string userId)
        {
            var nameParam = new SqlParameter("@name", name);
            var groupParam = new SqlParameter("@group", group);
            var groupImageParam = new SqlParameter("@roomImage", System.Data.SqlDbType.VarBinary, -1);
            groupImageParam.Value = DBNull.Value;

            if (image != null)
            {
               byte[] decodedByteArray = Convert.FromBase64CharArray(image.ToCharArray(), 0, image.Length);
                groupImageParam.Value = decodedByteArray;
            }



            var result = RawSqlQuery<ReturnGroupViewModel>.query("CREATE_MESSAGE_ROOM @name, @group, @roomImage", nameParam, groupParam, groupImageParam); //run stored procedure
      
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

            foreach(ReturnGroupViewModel model in result)
            {
                groupObject.id = model.id;
                groupObject.name = model.name;
                groupObject.group = model.group;
                groupObject.message = model.message;
                groupObject.group = model.group;
                groupObject.createdAt = model.createdAt;
                groupObject.lastUpdated = model.lastUpdated;
                
            
                string encodedByteArray = null;

                if (model.image != null)
                {
                    encodedByteArray = Convert.ToBase64String(model.image);
                }

                groupObject.image = encodedByteArray;
                
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

            return groupObject;

        }

        //saves message that was sent in real time in the controller.
        public bool SendMessage(SendTextViewModel textInfo, String user_id)
        {
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

            if (textInfo.image != null)
            {
                byte[] decodedByteArray = Convert.FromBase64CharArray(textInfo.image.ToCharArray(), 0, textInfo.image.Length);
                imageParam.Value = decodedByteArray;
            }


            var result = RawSqlQuery<SendTextViewModel>.query("INSERT_MESSAGE @_id, @room_id, @text, @createdAt, @user_id, @image, @video, @audio, @system, @sent, @received, @pending",
                idParam, roomIdParam, textParam, createdAtParam, userIdParam, imageParam, videoParam, audioParam, systemParam, sentParam, receivedParam, pendingParam); //run stored procedure

            var UpdateRoomIdParam = new SqlParameter("@room_id", textInfo.room_id);
            var updateRoom = RawSqlQuery<SendTextViewModel>.query("UPDATE_ROOM  @room_id", UpdateRoomIdParam); //run stored procedure

            bool returnAnswer = true;

            if (result == null)
            {
                returnAnswer = false;
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return returnAnswer;

        }

        
        //stores a room id along with a user id in order to create an association between the two
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

        //stores the a users expo token for push notification
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

        //gets a users expo token for push notification
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
        //deletes a users expo token
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
