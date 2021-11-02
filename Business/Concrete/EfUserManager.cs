using Business.Abstract;
using Business.Constants.Message;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using Core.Utilities.Results.DataResultOptions.DataOption;
using Core.Utilities.Results.ResultOptions.Option;
using DataAccess.Abstract;
using Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class EfUserManager : IUserService
    {
        IUserDal _userDal;

        public EfUserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        [ValidationAspect(typeof(UserValidator))]
        public IResult Add(User user)
        {
            IResult result = BusinessRules.Run(ChechIfUserAlreadyExists(user.UserId), CheckIfSameEmail(user.Email));
            if (result != null)
            {
                return new ErrorResult();
            }
            _userDal.Add(user);
            return new SuccessResult(Messages.UserAdded);
        }

        public IResult Delete(User user)
        {
            _userDal.Delete(user);
            return new SuccessResult(Messages.UserDeleted);
        }

        public IDataResult<List<User>> GetAll()
        {
            if (DateTime.Now.Hour == 1)
            {
                return new ErrorDataResult<List<User>>(_userDal.GetAll(), Messages.MaintainanceTimeUser);
            }
            return new SuccessDataResult<List<User>>(_userDal.GetAll(), Messages.UsersListed);
        }

        public IDataResult<List<User>> GetById(int userId)
        {
            return new SuccessDataResult<List<User>>(_userDal.GetAll(u => u.UserId == userId), Messages.UsersListedById);
        }

        [ValidationAspect(typeof(UserValidator))]
        public IResult Update(User user)
        {
            IResult result = BusinessRules.Run(ChechIfUserAlreadyExists(user.UserId), CheckIfSameEmail(user.Email));
            if (result != null)
            {
                return new ErrorResult();
            }
            _userDal.Update(user);
            return new SuccessResult(Messages.UserUpdated);
        }


        //////////////////// Business Rules //////////////////////////////


        private IResult ChechIfUserAlreadyExists(int userId)
        {
            var result = _userDal.GetAll(u => u.UserId == userId).Any();
            if (result != null)
            {
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            return new SuccessResult();
        }

        private IResult CheckIfSameEmail(string email)
        {
            var result = _userDal.GetAll(u => u.Email == email).Any();
            if (result != null)
            {
                return new ErrorResult(Messages.MailBeingUsedBySomeoneElse);
            }
            return new SuccessResult();
        }
    }
}
