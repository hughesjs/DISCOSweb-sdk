using System.Text;
using DISCOSweb_Sdk.Exceptions;
using DISCOSweb_Sdk.Extensions;
using DISCOSweb_Sdk.Queries.Filters;
using DISCOSweb_Sdk.Queries.Filters.Tree;

namespace DISCOSweb_Sdk.Queries;

public class DiscosQueryBuilder<TObject>: IDiscosQueryBuilder<TObject>
{
	private FilterTree _filterDefinitions = new();
	private List<string> _includes = new();
	private int? _numPages;
	private int? _pageNum;

	public DiscosQueryBuilder()
	{
		Reset();
	}

	public IDiscosQueryBuilder<TObject> AddFilter(FilterDefinition filterDefinition)
	{
		throw new NotImplementedException();
		return this;
	}

	public IDiscosQueryBuilder<TObject> AddInclude(string fieldName)
	{
		typeof(TObject).EnsureFieldExists(fieldName);
		_includes.Add(fieldName);
		return this;
	}

	public IDiscosQueryBuilder<TObject> AddAllIncludes() => throw new NotImplementedException();

	public IDiscosQueryBuilder<TObject> AddPageSize(int numPages)
	{
		_numPages = numPages;
		return this;
	}

	public IDiscosQueryBuilder<TObject> PageNum(int pageNum)
	{
		_pageNum = pageNum;
		return this;
	}

	public IDiscosQueryBuilder<TObject> Reset()
	{
		_filterDefinitions = new();
		_includes = new();
		_numPages = null;
		_pageNum = null;
		return this;
	}

	public string GetQueryString()
	{
		void AppendJoiningChar(StringBuilder builder, ref bool isFirst)
		{
			builder.Append(isFirst ? '?' : '&');
			isFirst = false;
		}
		StringBuilder builder = new();
		bool isFirst = true;
		if (_filterDefinitions.CountNodes() > 0)
		{
			AppendJoiningChar(builder, ref isFirst);
			AddFilterString(builder);
		}

		if (_includes.Count > 0)
		{
			AppendJoiningChar(builder, ref isFirst);
			AddIncludeString(builder);
		}

		if (_pageNum is not null)
		{
			AppendJoiningChar(builder, ref isFirst);
			AddPageNumber(builder);
		}

		if (_numPages is not null)
		{
			AppendJoiningChar(builder, ref isFirst);
			AddNumPages(builder);
		}
		return builder.ToString();
	}
	
	private void AddFilterString(StringBuilder builder)
	{
		builder.Append("filter=");
		throw new NotImplementedException();
	}

	private void AddIncludeString(StringBuilder builder)
	{
		builder.Append("include=");
		_includes.ForEach(i => builder.Append(i));
	}

	private void AddPageNumber(StringBuilder builder)
	{
		builder.Append($"page[number]={_pageNum}");
	}

	private void AddNumPages(StringBuilder builder)
	{
		builder.Append($"page[size]={_numPages}");
	}
}


public interface IDiscosQueryBuilder<TObject>
{
	public IDiscosQueryBuilder<TObject> AddFilter(FilterDefinition filterDefinition);
	public IDiscosQueryBuilder<TObject> AddInclude(string fieldName);
	public IDiscosQueryBuilder<TObject> AddAllIncludes();
	public IDiscosQueryBuilder<TObject> AddPageSize(int numPages);
	public IDiscosQueryBuilder<TObject> PageNum(int pageNum);
	public IDiscosQueryBuilder<TObject> Reset();
	public string GetQueryString();
}

