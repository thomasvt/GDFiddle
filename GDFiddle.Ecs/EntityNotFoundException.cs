﻿namespace GDFiddle.Ecs
{
    public class EntityNotFoundException
        : Exception
    {
        public EntityNotFoundException(string message)
        : base(message)
        {
            
        }
    }
}
