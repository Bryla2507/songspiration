using AutoMapper;
using SongSpiration.DAL.Interfaces;
using SongSpiration.Models.Entities;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;
using System.Linq.Expressions;

namespace SongSpiration.BLL.Services
{
    public class FilterService : IFilterService
    {
        private readonly IPinRepository _pinRepository;
        private readonly IMapper _mapper;

        public FilterService(IPinRepository pinRepository, IMapper mapper)
        {
            _pinRepository = pinRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PinDto>> FilterPins(PinFilterDto filterDto)
        {
            Expression<Func<Pin, bool>> filterExpression = pin => true;

            if (!string.IsNullOrEmpty(filterDto.Genre))
            {
                filterExpression = filterExpression.And(pin => pin.Genres.Any(g => g.Name == filterDto.Genre));
            }

            if (filterDto.Instrument != Instrument.Unknown)
            {
                filterExpression = filterExpression.And(pin => pin.Instrument == filterDto.Instrument);
            }

            if (filterDto.Visibility != PinVisibility.Unknown)
            {
                filterExpression = filterExpression.And(pin => pin.Visibility == filterDto.Visibility);
            }

            var pins = await _pinRepository.GetAll(filterExpression);
            return _mapper.Map<IEnumerable<PinDto>>(pins);
        }
    }

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }
    }

    public class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}