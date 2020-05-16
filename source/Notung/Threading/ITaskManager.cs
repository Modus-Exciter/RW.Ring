using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Threading
{
  public interface ITaskManager
  {
  }

  public class TaskManager : MarshalByRefObject, ITaskManager
  {
    private readonly ITaskManagerView m_view;

    public TaskManager(ITaskManagerView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    internal TaskManager() : this(new TaskManagerViewStub()) { }
  }

  public interface ITaskManagerView : ISynchronizeProvider
  {
  }

  public class TaskManagerViewStub : SynchronizeProviderStub, ITaskManagerView
  {
  }
}
