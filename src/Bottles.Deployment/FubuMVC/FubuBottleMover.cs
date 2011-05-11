using System;

namespace Bottles.Deployment.FubuMVC
{
    public class FubuBottleMover : IBottleMover
    {
        private readonly IBottleRepository _repository;

        public FubuBottleMover(IBottleRepository repository)
        {
            _repository = repository;
        }

        public void Move(BottleMoverRequest request)
        {
            throw new NotImplementedException();
        }
    }
}