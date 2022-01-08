using System.Drawing;
using System.Windows;
using Notung.ComponentModel;
using Notung.Feuerzauber.Properties;

namespace Notung.Feuerzauber.Dialogs
{
  sealed class MessageDialogContext : ObservableObject
  {
    private string m_message;
    private string m_title = string.Empty;
    private MessageBoxImage m_image;
    private MessageBoxButton m_buttons;
    private Bitmap m_bitmap;

    public string Message
    {
      get { return m_message; }
      set
      {
        if (object.Equals(m_message, value))
          return;

        m_message = value;
        this.OnPropertyChanged("Message");
      }
    }

    public string Title
    {
      get { return m_title; }
      set
      {
        if (object.Equals(m_title, value))
          return;

        m_title = value;
        this.OnPropertyChanged("Title");
      }
    }

    public MessageBoxImage Image
    {
      get { return m_image; }
      set
      {
        if (m_image == value)
          return;

        m_image = value;

        this.OnPropertyChanged("Image");

        if (m_bitmap == null)
          this.ExplicitBitmap = this.SelectBitmap();
      }
    }

    public MessageBoxButton Buttons
    {
      get { return m_buttons; }
      set
      {
        if (m_buttons == value)
          return;

        m_buttons = value;
        this.OnPropertyChanged("Buttons");
      }
    }

    public Bitmap ExplicitBitmap
    {
      get { return m_bitmap; }
      set
      {
        if (object.Equals(m_bitmap, value))
          return;

        m_bitmap = value;
        this.OnPropertyChanged("ExplicitBitmap");
      }
    }

    private Bitmap SelectBitmap()
    {
      switch (m_image)
      {
        case MessageBoxImage.Error:
          return Resources.Error_48;

        case MessageBoxImage.Warning:
          return Resources.Warning_48;

        case MessageBoxImage.Information:
          return Resources.Info_48;

        case MessageBoxImage.Question:
          return Resources.Question_48;

        default:
          return null;
      }
    }
  }
}