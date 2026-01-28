using StronglyTypedIds;

namespace TestApp.Domain;

[StronglyTypedId]
public partial record OrderId;

[StronglyTypedId]
public partial record CustomerId;

[StronglyTypedId]
public partial record CardId;

[StronglyTypedId(IdType.Long)]
public partial record ItemId;