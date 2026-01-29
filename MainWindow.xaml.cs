using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace JournalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

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

        public ObservableArticle CurrentArticle = new ObservableArticle
        {
            Article = new Article()
        };

        public refreshObservable refreshNotifier = new refreshObservable();

        string connectionString = ConfigurationManager.ConnectionStrings["JournalDB"].ConnectionString;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM articles INNER JOIN journals ON articles.journal_id = journals.journal_id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    DataTable dt = new DataTable("authors");
                    sda.Fill(dt);
                    sda.FillSchema(dt, SchemaType.Source);
                    dt.Columns.Add("PageRange", typeof(string), "page_start+ ' - ' +page_end");

                    AllArticlesDataGrid.ItemsSource = dt.DefaultView;
                    AllArticlesDataGrid.AutoGenerateColumns = false;
                    AllArticlesDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Article Title",
                        Binding = new Binding("article_title"),
                        Width = 400,
                        ElementStyle = new Style(typeof(TextBlock))
                        {
                            Setters =
                            {
                                new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap)
                            }
                        }
                    });
                    AllArticlesDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Number",
                        Binding = new Binding("journal_number"),
                        ElementStyle = new Style(typeof(TextBlock))
                        {
                            Setters =
                            {
                                new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                                new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                            }
                        }
                    });
                    AllArticlesDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Pages",
                        Binding = new Binding("PageRange"),
                        ElementStyle = new Style(typeof(TextBlock))
                        {
                            Setters =
                            {
                                new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                                new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                            }
                        }
                    });
                    
                    
                    authorsArticles.ItemsSource = dt.DefaultView;
                    authorsArticles.AutoGenerateColumns = false;
                    authorsArticles.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Article Title",
                        Binding = new Binding("article_title"),
                        Width = 300,
                        ElementStyle = new Style(typeof(TextBlock))
                        {
                            Setters =
                            {
                                new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis)
                            }
                        }
                    });
                    authorsArticles.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Pages",
                        Binding = new Binding("PageRange")
                    });

                    articleAuthors.AutoGenerateColumns = false;

                    string sql2 = "SELECT * FROM journals";
                    SqlCommand command2 = new SqlCommand(sql2, connection);
                    SqlDataAdapter sda2 = new SqlDataAdapter(command2);
                    DataTable dt2 = new DataTable("journals");
                    sda2.Fill(dt2);
                    journalDataGrid.ItemsSource = dt2.DefaultView;

                    JournalsCombobox.ItemsSource = dt2.DefaultView;
                    JournalsCombobox.DisplayMemberPath = "journal_number";
                    JournalsCombobox.SelectedValuePath = "journal_id";

                    string sql3 = "SELECT * FROM articles WHERE journal_id=1 ORDER BY page_start";
                    SqlCommand command3 = new SqlCommand(sql3, connection);
                    SqlDataAdapter sda3 = new SqlDataAdapter(command3);
                    DataTable dt3 = new DataTable("articles");
                    sda3.Fill(dt3);
                    issueDataGrid.ItemsSource = dt3.DefaultView;

                    string sql4 = "SELECT * FROM authors";
                    SqlCommand command4 = new SqlCommand(sql4, connection);
                    SqlDataAdapter sda4 = new SqlDataAdapter(command4);
                    DataTable dt4 = new DataTable("authors");
                    sda4.Fill(dt4);
                    dataGrid2.ItemsSource = dt4.DefaultView;

                    var authorComboboxDict = new Dictionary<int, string>();
                    foreach (DataRow row in dt4.Rows)
                    {
                        authorComboboxDict.Add(Convert.ToInt32(row["author_id"]), row["first_name"] + " " + row["last_name"]);
                    }
                    AuthorsCombobox.ItemsSource = authorComboboxDict;
                    AuthorsCombobox.DisplayMemberPath = "Value";
                    AuthorsCombobox.SelectedValuePath = "Key";

                    refreshNotifier.refreshNeeded = false;

                    refreshNotifier.RefreshRequested += (s, args) =>
                    {
                        if (args.refreshTable == "articles" && args.refreshNeeded)
                        {
                            using (SqlConnection refreshConnection = new SqlConnection(connectionString))
                            {
                                try
                                {
                                    CurrentArticle.DeselectArticle();

                                    refreshConnection.Open();
                                    string refreshSql = "SELECT * FROM articles INNER JOIN journals ON articles.journal_id = journals.journal_id";
                                    SqlCommand refreshCommand = new SqlCommand(refreshSql, refreshConnection);
                                    SqlDataAdapter refreshSda = new SqlDataAdapter(refreshCommand);
                                    DataTable refreshDt = new DataTable("articles");
                                    refreshSda.Fill(refreshDt);
                                    refreshDt.Columns.Add("PageRange", typeof(string), "page_start+ ' - ' +page_end");
                                    AllArticlesDataGrid.ItemsSource = refreshDt.DefaultView;
                                    authorsArticles.ItemsSource = refreshDt.DefaultView;

                                    refreshNotifier.RefreshComplete();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error refreshing articles: " + ex.Message);
                                }
                            }
                        }
                    };

                    CurrentArticle.ArticleChanged += (s, args) =>
                    {
                        if (!CurrentArticle.Selected)
                        {
                            EditArticlePanel.Visibility = Visibility.Collapsed;
                            CreateNewArticlePanel.Visibility = Visibility.Visible;
                        } else
                        {
                            EditArticlePanel.Visibility = Visibility.Visible;
                            CreateNewArticlePanel.Visibility = Visibility.Collapsed;
                        }

                        ArticleTitleTextBox.Text = CurrentArticle.Article.ArticleTitle;
                        ArticlePageStart.Text = CurrentArticle.Article.PageStart.ToString();
                        ArticlePageEnd.Text = CurrentArticle.Article.PageEnd.ToString();
                        JournalsCombobox.SelectedValue = CurrentArticle.Article.JournalId;

                        if (CurrentArticle.Article.ArticleId == 0)
                        {
                            ArticleEditButton.Visibility = Visibility.Collapsed;
                            ArticleDeleteButton.Visibility = Visibility.Collapsed;
                            ArticleCreateButton.Content = "Create";
                            articleAuthors.ItemsSource = null;
                            return;
                        } else
                        {
                            ArticleEditButton.Visibility = Visibility.Visible;
                            ArticleDeleteButton.Visibility = Visibility.Visible;
                            ArticleCreateButton.Content = "Add new";
                        }

                        using (SqlConnection ArAuConnection = new SqlConnection(connectionString))
                        {
                            ArAuConnection.Open();

                            string authorsOfArticleSql = "SELECT * FROM authors WHERE author_id IN (SELECT author_id FROM article_authors WHERE article_id = @article_id)";
                            SqlCommand authorsOfArticle = new SqlCommand(authorsOfArticleSql, ArAuConnection);
                            SqlDataAdapter ArAuSda = new SqlDataAdapter(authorsOfArticle);
                            authorsOfArticle.Parameters.AddWithValue("@article_id", CurrentArticle.Article.ArticleId);
                            DataTable ArAuDt = new DataTable("authors");
                            ArAuSda.Fill(ArAuDt);
                            articleAuthors.ItemsSource = ArAuDt.DefaultView;
                        }
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database connection Error: " + ex.Message);
                }
            }
        }

        private void articleAuthors_AutoGeneratedColumns(object sender, EventArgs e)
        {

        }

        private void ArticleSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)AllArticlesDataGrid.SelectedItem;

            if (selectedRow != null) {
                CurrentArticle.UpdateArticle(
                    Convert.ToInt32(selectedRow["article_id"]),
                    selectedRow["article_title"].ToString(),
                    Convert.ToInt32(selectedRow["page_start"]),
                    Convert.ToInt32(selectedRow["page_end"]),
                    Convert.ToInt32(selectedRow["journal_id"])
                );
            }

        }

        private void CreateNewArticleButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentArticle.UpdateArticle(0, String.Empty, 0, 0, 0);
        }

        private void ArticleCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentArticle.DeselectArticle();
        }

        private void ArticleCreateButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "INSERT INTO articles (article_title, page_start, page_end, journal_id) VALUES (@article_title, @page_start, @page_end, @journal_id)";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@article_title", ArticleTitleTextBox.Text);
                    command.Parameters.AddWithValue("@page_start", Convert.ToInt32(ArticlePageStart.Text));
                    command.Parameters.AddWithValue("@page_end", Convert.ToInt32(ArticlePageEnd.Text));
                    command.Parameters.AddWithValue("@journal_id", Convert.ToInt32(JournalsCombobox.SelectedValue));
                    command.ExecuteNonQuery();

                    refreshNotifier.RequestRefresh("articles");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating article: " + ex.Message);
                }
            }
        }

        private void ArticleEditButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "UPDATE articles SET article_title=@article_title, page_start=@page_start, page_end=@page_end, journal_id=@journal_id WHERE article_id=@article_id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@article_title", ArticleTitleTextBox.Text);
                    command.Parameters.AddWithValue("@page_start", Convert.ToInt32(ArticlePageStart.Text));
                    command.Parameters.AddWithValue("@page_end", Convert.ToInt32(ArticlePageEnd.Text));
                    command.Parameters.AddWithValue("@journal_id", Convert.ToInt32(JournalsCombobox.SelectedValue));
                    command.Parameters.AddWithValue("@article_id", CurrentArticle.Article.ArticleId);
                    command.ExecuteNonQuery();

                    refreshNotifier.RequestRefresh("articles");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error editing article: " + ex.Message);
                }
            }
        }

        private void ArticleDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the article \"" + CurrentArticle.Article.ArticleTitle + "\"?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    connection.Open();
                    string sql = "DELETE FROM articles WHERE article_id=@article_id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@article_id", CurrentArticle.Article.ArticleId);
                    command.ExecuteNonQuery();

                    refreshNotifier.RequestRefresh("articles");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting article: " + ex.Message);
                }
            }
        }

        private void AuthorsCombobox_TextInput(object sender, TextCompositionEventArgs e)
        {
            string searchText = AuthorsCombobox.Text.ToLower();
            AuthorsCombobox.Items.Filter = item =>
            {
                var pair = (KeyValuePair<int, string>)item;
                return pair.Value.ToLower().Contains(searchText);
            };
        }

        private void ArticleRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            refreshNotifier.RequestRefresh("articles");
        }
    }
}
