
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;


namespace TARge25Shop.Core.ServiceInterface
{
    public interface IKindergartenServices
    {
        Task<Kindergarten> Create(KindergartenDto dto);
        Task<Kindergarten> Update(KindergartenDto dto);
        Task<Kindergarten> DetailAsync(Guid id);
        Task<Kindergarten> Delete(Guid id);
    }
}
