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

        public ObservableArticle CurrentArticle = new ObservableArticle
        {
            Article = new Article()
        };
        public ObservableAuthor CurrentAuthor = new ObservableAuthor
        {
            Author = new Author()
        };
        public ObservableJournal CurrentJournal = new ObservableJournal
        {
            Journal = new Journal()
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
                    string sql = "SELECT * FROM articles INNER JOIN journals ON articles.journal_id = journals.journal_id INNER JOIN article_authors ON articles.article_id = article_authors.article_id INNER JOIN authors ON article_authors.author_id = authors.author_id where article_authors.author_order=1;";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    DataTable dt = new DataTable("authors");
                    sda.Fill(dt);
                    sda.FillSchema(dt, SchemaType.Source);
                    dt.Columns.Add("PageRange", typeof(string), "page_start+ ' - ' +page_end");
                    dt.Columns.Add("AuthorFullName", typeof(string), "first_name + ' ' + last_name");

                    AllArticlesDataGrid.ItemsSource = dt.DefaultView;
                    AllArticlesDataGrid.AutoGenerateColumns = false;
                    AllArticlesDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Article Title",
                        Binding = new Binding("article_title"),
                        Width = 433,
                        ElementStyle = new Style(typeof(TextBlock))
                        {
                            Setters =
                            {
                                new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap),
                                new Setter(TextBlock.PaddingProperty, new Thickness(5,5,0,5))
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
                        Width = 60,
                        MinWidth = 60,
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
                        Header = "Author",
                        Binding = new Binding("AuthorFullName"),
                        Width = 200,
                        ElementStyle = new Style(typeof(TextBlock))
                        {
                            Setters =
                            {
                                new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis),
                                new Setter(TextBlock.PaddingProperty, new Thickness(5,5,0,5))
                            }
                        }
                    });

                    authorsArticles.AutoGenerateColumns = false;
                    authorsArticles.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Article Title",
                        Binding = new Binding("article_title"),
                        Width = 370,
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

                    journalDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Journal Number",
                        Binding = new Binding("journal_number"),
                    });
                    journalDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Publication Year",
                        Binding = new Binding("publication_year"),
                    });

                    JournalsCombobox.ItemsSource = dt2.DefaultView;
                    JournalsCombobox.DisplayMemberPath = "journal_number";
                    JournalsCombobox.SelectedValuePath = "journal_id";

                    issueDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Article Title",
                        Binding = new Binding("article_title"),
                        Width = 875,
                        ElementStyle = new Style(typeof(TextBlock))
                        {
                            Setters =
                            {
                                new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap)
                            }
                        }
                    });
                    issueDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Pages",
                        Binding = new Binding("PageRange"),
                        Width = 60,
                        MinWidth = 60,
                        ElementStyle = new Style(typeof(TextBlock))
                        {
                            Setters =
                            {
                                new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
                                new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                            }
                        }
                    });

                    string sql4 = "SELECT * FROM authors";
                    SqlCommand command4 = new SqlCommand(sql4, connection);
                    SqlDataAdapter sda4 = new SqlDataAdapter(command4);
                    DataTable dt4 = new DataTable("authors");
                    sda4.Fill(dt4);
                    
                    allAuthorsDatagrid.ItemsSource = dt4.DefaultView;
                    allAuthorsDatagrid.AutoGenerateColumns = false;
                    allAuthorsDatagrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "First Name",
                        Binding = new Binding("first_name"),
                        Width = 150
                    });
                    allAuthorsDatagrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = "Last Name",
                        Binding = new Binding("last_name"),
                        Width = 150
                    });

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
                        if (args.refreshNeeded)
                        {
                            switch (args.refreshTable)
                            {
                                case "articles":
                                    using (SqlConnection refreshConnection = new SqlConnection(connectionString))
                                    {
                                        try
                                        {
                                            CurrentArticle.DeselectArticle();

                                            refreshConnection.Open();
                                            string refreshSql = "SELECT * FROM articles INNER JOIN journals ON articles.journal_id = journals.journal_id INNER JOIN article_authors ON articles.article_id = article_authors.article_id INNER JOIN authors ON article_authors.author_id = authors.author_id where article_authors.author_order=1;";
                                            SqlCommand refreshCommand = new SqlCommand(refreshSql, refreshConnection);
                                            SqlDataAdapter refreshSda = new SqlDataAdapter(refreshCommand);
                                            DataTable refreshDt = new DataTable("articles");
                                            refreshSda.Fill(refreshDt);
                                            refreshDt.Columns.Add("PageRange", typeof(string), "page_start+ ' - ' +page_end");
                                            refreshDt.Columns.Add("AuthorFullName", typeof(string), "first_name + ' ' + last_name");
                                            AllArticlesDataGrid.ItemsSource = refreshDt.DefaultView;

                                            refreshNotifier.RefreshComplete();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Error refreshing articles: " + ex.Message);
                                        }
                                    }
                                    break;
                                case "authors":
                                    using (SqlConnection refreshConnection = new SqlConnection(connectionString))
                                    {
                                        try
                                        {
                                            CurrentAuthor.DeselectAuthor();
                                            refreshConnection.Open();
                                            string refreshSql = "SELECT * FROM authors";
                                            SqlCommand refreshCommand = new SqlCommand(refreshSql, refreshConnection);
                                            SqlDataAdapter refreshSda = new SqlDataAdapter(refreshCommand);
                                            DataTable refreshDt = new DataTable("authors");
                                            refreshSda.Fill(refreshDt);
                                            allAuthorsDatagrid.ItemsSource = refreshDt.DefaultView;
                                            authorComboboxDict.Clear();
                                            AuthorsCombobox.ItemsSource = null;
                                            foreach (DataRow row in refreshDt.Rows)
                                            {
                                                authorComboboxDict.Add(Convert.ToInt32(row["author_id"]), row["first_name"] + " " + row["last_name"]);
                                            }
                                            AuthorsCombobox.ItemsSource = authorComboboxDict;

                                            refreshNotifier.RefreshComplete();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Error refreshing authors: " + ex.Message);
                                        }
                                    }
                                    break;
                                case "journals":
                                    using (SqlConnection refreshConnection = new SqlConnection(connectionString))
                                    {
                                        try
                                        {
                                            CurrentJournal.DeselectJournal();
                                            refreshConnection.Open();
                                            string refreshSql = "SELECT * FROM journals";
                                            SqlCommand refreshCommand = new SqlCommand(refreshSql, refreshConnection);
                                            SqlDataAdapter refreshSda = new SqlDataAdapter(refreshCommand);
                                            DataTable refreshDt = new DataTable("journals");
                                            refreshSda.Fill(refreshDt);
                                            journalDataGrid.ItemsSource = refreshDt.DefaultView;
                                            JournalsCombobox.ItemsSource = refreshDt.DefaultView;
                                            refreshNotifier.RefreshComplete();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Error refreshing journals: " + ex.Message);
                                        }
                                    }
                                    break;
                            }
                        }
                    };

                    CurrentArticle.ArticleChanged += (s, args) =>
                    {
                        if (!CurrentArticle.Selected)
                        {
                            EditArticlePanel.Visibility = Visibility.Collapsed;
                            CreateNewArticlePanel.Visibility = Visibility.Visible;
                            return;
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

                    CurrentAuthor.AuthorChanged += (s, args) =>
                    {
                        if (!CurrentAuthor.Selected)
                        {
                            EditAuthorPanel.Visibility = Visibility.Collapsed;
                            CreateNewAuthorPanel.Visibility = Visibility.Visible;
                            return;
                        }
                        else
                        {
                            EditAuthorPanel.Visibility = Visibility.Visible;
                            CreateNewAuthorPanel.Visibility = Visibility.Collapsed;
                        }

                        authorFirstNameTextbox.Text = CurrentAuthor.Author.FirstName;
                        authorLastNameTextbox.Text = CurrentAuthor.Author.LastName;

                        if (CurrentAuthor.Author.AuthorId == 0)
                        {
                            AuthorEditButton.Visibility = Visibility.Collapsed;
                            AuthorDeleteButton.Visibility = Visibility.Collapsed;
                            AuthorCreateButton.Content = "Create";
                            authorsArticles.ItemsSource = null;
                            return;
                        }
                        else
                        {
                            AuthorEditButton.Visibility = Visibility.Visible;
                            AuthorDeleteButton.Visibility = Visibility.Visible;
                            AuthorCreateButton.Content = "Add new";
                        }

                        using (SqlConnection AuArConnection = new SqlConnection(connectionString))
                        {
                            AuArConnection.Open();
                            string articlesOfAuthorSql = "SELECT * FROM articles INNER JOIN journals ON articles.journal_id = journals.journal_id WHERE article_id IN (SELECT article_id FROM article_authors WHERE author_id = @author_id)";
                            SqlCommand articlesOfAuthor = new SqlCommand(articlesOfAuthorSql, AuArConnection);
                            SqlDataAdapter AuArSda = new SqlDataAdapter(articlesOfAuthor);
                            articlesOfAuthor.Parameters.AddWithValue("@author_id", CurrentAuthor.Author.AuthorId);
                            DataTable AuArDt = new DataTable("articles");
                            AuArSda.FillSchema(AuArDt, SchemaType.Source);
                            AuArSda.Fill(AuArDt);
                            AuArDt.Columns.Add("PageRange", typeof(string), "page_start+ ' - ' +page_end");
                            authorsArticles.ItemsSource = AuArDt.DefaultView;
                        }

                    };

                    CurrentJournal.JournalChanged += (s, args) =>
                    {
                        if (!CurrentJournal.Selected)
                        {
                            EditJournalPanel.Visibility = Visibility.Collapsed;
                            CreateNewJournalPanel.Visibility = Visibility.Visible;
                            issueDataGrid.ItemsSource = null;
                            return;
                        }
                        else
                        {
                            EditJournalPanel.Visibility = Visibility.Visible;
                            CreateNewJournalPanel.Visibility = Visibility.Collapsed;
                        }

                        journalNumberTextBox.Text = CurrentJournal.Journal.JournalNumber.ToString();
                        publicationYearTextBox.Text = CurrentJournal.Journal.PublicationYear.ToString();

                        if (CurrentJournal.Journal.JournalId == 0)
                        {
                            JournalEditButton.Visibility = Visibility.Collapsed;
                            JournalDeleteButton.Visibility = Visibility.Collapsed;
                            JournalCreateButton.Content = "Create";
                            issueDataGrid.ItemsSource = null;
                            return;
                        }
                        else
                        {
                            JournalEditButton.Visibility = Visibility.Visible;
                            JournalDeleteButton.Visibility = Visibility.Visible;
                            JournalCreateButton.Content = "Add new";
                        }

                        using (SqlConnection JoArConnection = new SqlConnection(connectionString))
                        {
                            JoArConnection.Open();
                            string articlesOfJournalSql = "SELECT * FROM articles WHERE journal_id = @journal_id ORDER BY page_start";
                            SqlCommand articlesOfJournal = new SqlCommand(articlesOfJournalSql, JoArConnection);
                            SqlDataAdapter JoArSda = new SqlDataAdapter(articlesOfJournal);
                            articlesOfJournal.Parameters.AddWithValue("@journal_id", CurrentJournal.Journal.JournalId);
                            DataTable JoArDt = new DataTable("articles");
                            JoArSda.FillSchema(JoArDt, SchemaType.Source);
                            JoArSda.Fill(JoArDt);
                            JoArDt.Columns.Add("PageRange", typeof(string), "page_start+ ' - ' +page_end");
                            issueDataGrid.ItemsSource = JoArDt.DefaultView;
                        }
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database connection Error: " + ex.StackTrace);
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

        private void dataGrid2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)allAuthorsDatagrid.SelectedItem;

            if (selectedRow != null) {
                CurrentAuthor.UpdateAuthor(
                    Convert.ToInt32(selectedRow["author_id"]),
                    selectedRow["first_name"].ToString(),
                    selectedRow["last_name"].ToString()
                );
            }
        }

        private void CreateNewAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentAuthor.UpdateAuthor(0, String.Empty, String.Empty);
        }

        private void AuthorCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentAuthor.DeselectAuthor();
        }

        private void AuthorDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the author \"" + CurrentAuthor.Author.FirstName + " " + CurrentAuthor.Author.LastName + "\"?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    connection.Open();
                    string sql = "DELETE FROM authors WHERE author_id=@author_id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@author_id", CurrentAuthor.Author.AuthorId);
                    command.ExecuteNonQuery();
                    refreshNotifier.RequestRefresh("authors");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting author: " + ex.Message);
                }
            }
        }

        private void AuthorEditButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "UPDATE authors SET first_name=@first_name, last_name=@last_name WHERE author_id=@author_id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@first_name", authorFirstNameTextbox.Text);
                    command.Parameters.AddWithValue("@last_name", authorLastNameTextbox.Text);
                    command.Parameters.AddWithValue("@author_id", CurrentAuthor.Author.AuthorId);
                    command.ExecuteNonQuery();
                    refreshNotifier.RequestRefresh("authors");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error editing author: " + ex.Message);
                }
            }
        }

        private void AuthorCreateButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "INSERT INTO authors (first_name, last_name) VALUES (@first_name, @last_name)";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@first_name", authorFirstNameTextbox.Text);
                    command.Parameters.AddWithValue("@last_name", authorLastNameTextbox.Text);
                    command.ExecuteNonQuery();
                    refreshNotifier.RequestRefresh("authors");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating author: " + ex.Message);
                }
            }
        }

        private void AuthorRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            refreshNotifier.RequestRefresh("authors");
        }

        private void journalDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)journalDataGrid.SelectedItem;

            if (selectedRow != null)
            {
                CurrentJournal.UpdateJournal(
                    Convert.ToInt32(selectedRow["journal_id"]),
                    Convert.ToInt32(selectedRow["journal_number"]),
                    Convert.ToInt32(selectedRow["publication_year"])
                );
            }
        }

        private void CreateNewJournalButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentJournal.UpdateJournal(0, 0, 0);
        }

        private void JournalCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentJournal.DeselectJournal();
        }

        private void JournalCreateButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "INSERT INTO journals (journal_number, publication_year) VALUES (@journal_number, @publication_year)";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@journal_number", Convert.ToInt32(journalNumberTextBox.Text));
                    command.Parameters.AddWithValue("@publication_year", Convert.ToInt32(publicationYearTextBox.Text));
                    command.ExecuteNonQuery();
                    refreshNotifier.RequestRefresh("journals");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating journal: " + ex.Message);
                }
            }
        }

        private void JournalEditButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "UPDATE journals SET journal_number=@journal_number, publication_year=@publication_year WHERE journal_id=@journal_id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@journal_number", Convert.ToInt32(journalNumberTextBox.Text));
                    command.Parameters.AddWithValue("@publication_year", Convert.ToInt32(publicationYearTextBox.Text));
                    command.Parameters.AddWithValue("@journal_id", CurrentJournal.Journal.JournalId);
                    command.ExecuteNonQuery();
                    refreshNotifier.RequestRefresh("journals");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error editing journal: " + ex.Message);
                }
            }
        }

        private void JournalDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the journal issue \"" + CurrentJournal.Journal.JournalNumber + "\"?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    connection.Open();
                    string sql = "DELETE FROM journals WHERE journal_id=@journal_id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@journal_id", CurrentJournal.Journal.JournalId);
                    command.ExecuteNonQuery();
                    refreshNotifier.RequestRefresh("journals");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting journal: " + ex.Message);
                }
            }
        }

        private void JournalRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            refreshNotifier.RequestRefresh("journals");
        }
    }
}
