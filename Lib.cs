using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalProject
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int JournalId { get; set; }
    }

    public class Journal
    {
        public int JournalId { get; set; }
        public int JournalNumber { get; set; }
        public int PublicationYear { get; set; }
    }

    public class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ArticleEventArgs : EventArgs
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int JournalId { get; set; }
        public ArticleEventArgs(int articleId, string articleTitle, int pageStart, int pageEnd, int journalId)
        {
            articleId = ArticleId;
            articleTitle = ArticleTitle;
            pageStart = PageStart;
            pageEnd = PageEnd;
            journalId = JournalId;
        }
    }

    public class AuthorEventArgs : EventArgs
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AuthorEventArgs(int authorId, string firstName, string lastName)
        {
            authorId = AuthorId;
            firstName = FirstName;
            lastName = LastName;
        }
    }

    public class ObservableArticle
    {
        public Article Article { get; set; }
        public Boolean Selected { get; set; }

        public event EventHandler<ArticleEventArgs> ArticleChanged;

        public void UpdateArticle(int articleId, string articleTitle, int pageStart, int pageEnd, int journalId)
        {
            Selected = true;
            Article.ArticleId = articleId;
            Article.ArticleTitle = articleTitle;
            Article.PageStart = pageStart;
            Article.PageEnd = pageEnd;
            Article.JournalId = journalId;

            OnArticleChanged(new ArticleEventArgs(Article.ArticleId, articleTitle, pageStart, pageEnd, journalId));
        }

        public void UpdateArticle(Article article)
        {
            Selected = true;
            Article = article;
            OnArticleChanged(new ArticleEventArgs(Article.ArticleId, Article.ArticleTitle, Article.PageStart, Article.PageEnd, Article.JournalId));
        }

        public void DeselectArticle()
        {
            Selected = false;
            OnArticleChanged(new ArticleEventArgs(0, String.Empty, 0, 0, 0));
        }

        protected virtual void OnArticleChanged(ArticleEventArgs e)
        {
            ArticleChanged?.Invoke(this, e);
        }
    }

    public class ObservableAuthor
    {
        public Author Author { get; set; }
        public Boolean Selected { get; set; }

        public event EventHandler<AuthorEventArgs> AuthorChanged;

        public void UpdateAuthor(int authorId, string firstName, string lastName)
        {
            Selected = true;
            Author.AuthorId = authorId;
            Author.FirstName = firstName;
            Author.LastName = lastName;

            OnAuthorChanged(new AuthorEventArgs(Author.AuthorId, firstName, lastName));
        }

        public void DeselectAuthor()
        {
            Selected = false;
            OnAuthorChanged(new AuthorEventArgs(0, String.Empty, String.Empty));
        }

        protected virtual void OnAuthorChanged(AuthorEventArgs e)
        {
            AuthorChanged?.Invoke(this, e);
        }
    }

    public class refreshEventArgs : EventArgs
    {
        public string refreshTable { get; set; }
        public Boolean refreshNeeded { get; set; }
        public refreshEventArgs(string refreshTable, bool refreshNeeded)
        {
            this.refreshTable = refreshTable;
            this.refreshNeeded = refreshNeeded;
        }
    }

    public class refreshObservable
    {
        public Boolean refreshNeeded { get; set; }
        public string TableName { get; set; }

        public event EventHandler<refreshEventArgs> RefreshRequested;
        public void RequestRefresh(string table)
        {
            refreshNeeded = true;
            OnRefreshRequested(new refreshEventArgs(table, refreshNeeded));
        }
        public void RefreshComplete()
        {
            refreshNeeded = false;
        }
        protected virtual void OnRefreshRequested(refreshEventArgs e)
        {
            RefreshRequested?.Invoke(this, e);
        }
    }
}
