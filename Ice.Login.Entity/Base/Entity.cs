using System.ComponentModel.DataAnnotations;

namespace Ice.Login.Entity.Base;

public abstract class Entity
{
    [Key] public long Id { get; set; }
}