#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Crm;
using System.Collections.Generic;
using System.Linq;
using Gif.Service.Contracts;

namespace Gif.Service.Services
{
  public abstract class ServiceBase<T> : IDatastoreBase<T>
  {
    protected IRepository Repository;

    public ServiceBase(IRepository repository)
    {
      Repository = repository;
    }

    protected static List<TOther> GetInsertionTree<TOther>(List<TOther> allNodes) where TOther : IHasPreviousId
    {
      var roots = GetRoots(allNodes);
      var tree = new List<TOther>(roots);

      var next = GetChildren(roots, allNodes);
      while (next.Any())
      {
        tree.AddRange(next);
        next = GetChildren(next, allNodes);
      }

      return tree;
    }

    private static List<TOther> GetRoots<TOther>(List<TOther> allNodes) where TOther : IHasPreviousId
    {
      return allNodes.Where(x => x.PreviousId == null).ToList();
    }

    private static List<TOther> GetChildren<TOther>(List<TOther> parents, List<TOther> allNodes) where TOther : IHasPreviousId
    {
      return parents.SelectMany(parent => allNodes.Where(x => x.PreviousId == parent.Id)).ToList();
    }
  }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
