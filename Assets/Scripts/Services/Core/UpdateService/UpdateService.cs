using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Updater
{
    public class UpdateService : MonoBehaviour
    {
        private HashSet<IUpdatable> _updatables = new HashSet<IUpdatable>();
        private HashSet<ILateUpdatable> _lateUpdatables = new HashSet<ILateUpdatable>();
        private HashSet<IFixedUpdatable> _fixedUpdatables = new HashSet<IFixedUpdatable>();

        private HashSet<IBaseUpdatable> _toAddQueue = new HashSet<IBaseUpdatable>();
        private HashSet<IBaseUpdatable> _toRemoveQueue = new HashSet<IBaseUpdatable>();

        private bool _areUpdatablesLocked;

        public void Register(IBaseUpdatable updatable)
        {
            if (_areUpdatablesLocked)
            {
                _toAddQueue.Add(updatable);
                return;
            }

            if (updatable is IUpdatable)
            {
                _updatables.Add(updatable as IUpdatable);
            }

            if (updatable is ILateUpdatable)
            {
                _lateUpdatables.Add(updatable as ILateUpdatable);
            }

            if (updatable is IFixedUpdatable)
            {
                _fixedUpdatables.Add(updatable as IFixedUpdatable);
            }
        }

        public void Unregister(IBaseUpdatable updatable)
        {
            if (_areUpdatablesLocked)
            {
                _toRemoveQueue.Add(updatable);
                return;
            }

            if (updatable is IUpdatable)
            {
                _updatables.Remove(updatable as IUpdatable);
            }

            if (updatable is ILateUpdatable)
            {
                _lateUpdatables.Remove(updatable as ILateUpdatable);
            }

            if (updatable is IFixedUpdatable)
            {
                _fixedUpdatables.Remove(updatable as IFixedUpdatable);
            }
        }

        private void Update()
        {
            _areUpdatablesLocked = true;
            foreach (var updatable in _updatables)
            {
                updatable.Update();
            }

            AfterUpdate();
        }

        private void LateUpdate()
        {
            _areUpdatablesLocked = true;
            foreach (var updatable in _lateUpdatables)
            {
                updatable.LateUpdate();
            }

            AfterUpdate();
        }

        private void FixedUpdate()
        {
            _areUpdatablesLocked = true;
            foreach (var updatable in _fixedUpdatables)
            {
                updatable.FixedUpdate(Time.deltaTime);
            }

            AfterUpdate();
        }

        private void AfterUpdate()
        {
            _areUpdatablesLocked = false;
            ProcessQueue(_toAddQueue, Register);
            ProcessQueue(_toRemoveQueue, Unregister);
        }

        private void ProcessQueue(HashSet<IBaseUpdatable> queue, Action<IBaseUpdatable> action)
        {
            if (queue.Count == 0)
            {
                return;
            }

            foreach (var updatable in queue)
            {
                action(updatable);
            }

            queue.Clear();
        }
    }
}