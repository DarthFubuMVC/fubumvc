using Raven.Abstractions;
using Raven.Database.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Raven.Database.Linq.PrivateExtensions;
using Lucene.Net.Documents;
using System.Globalization;
using System.Text.RegularExpressions;
using Raven.Database.Indexing;


public class Index_Auto_2fUsers_2fByPasswordAndUserName : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fUsers_2fByPasswordAndUserName()
	{
		this.ViewText = @"from doc in docs.Users
select new { UserName = doc.UserName, Password = doc.Password }";
		this.ForEntityNames.Add("Users");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "Users", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				UserName = doc.UserName,
				Password = doc.Password,
				__document_id = doc.__document_id
			});
		this.AddField("UserName");
		this.AddField("Password");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("UserName");
		this.AddQueryParameterForMap("Password");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("UserName");
		this.AddQueryParameterForReduce("Password");
		this.AddQueryParameterForReduce("__document_id");
	}
}
