﻿using GitHubBot;

namespace Interfaces
{
    interface ITrigger<TContext>
    {
        public bool Condition(TContext obj);

        public void Action(TContext obj);
    }
}