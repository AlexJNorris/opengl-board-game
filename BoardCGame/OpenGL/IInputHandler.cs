using OpenTK.Input;
using Projeto_CG_1_GQ.Configuration.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Projeto_CG_1_GQ.Configuration
{
    public interface IInputHandler
    {
        ICamera Camera { get; }
        ICameraLookable Lookable { get; }

        void Handle(Key keys);
        void Execute();
    }
}
