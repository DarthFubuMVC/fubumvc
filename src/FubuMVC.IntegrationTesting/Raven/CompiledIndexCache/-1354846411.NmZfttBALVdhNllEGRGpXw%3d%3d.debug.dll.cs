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


public class Index_Auto_2fAudits_2fByTimestampSortByTimestamp : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fAudits_2fByTimestampSortByTimestamp()
	{
		this.ViewText = @"from doc in docs.Audits
select new { Timestamp = doc.Timestamp }";
		this.ForEntityNames.Add("Audits");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "Audits", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				Timestamp = doc.Timestamp,
				__document_id = doc.__document_id
			});
		this.AddField("Timestamp");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("Timestamp");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("Timestamp");
		this.AddQueryParameterForReduce("__document_id");
	}
}
