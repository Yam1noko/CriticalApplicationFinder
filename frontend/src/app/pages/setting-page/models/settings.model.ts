export interface Settings {
  Logging: {
    LogLevel: {
      Default: string;
      MicrosoftAspNetCore: string; // "Microsoft.AspNetCore"
    };
  };
  ConnectionStrings: {
    InternalDb: string;
    ExternalDb: string;
  };
  EmailSettings: {
    Host: string;
    Port: number;
    From: string;
    Username: string;
    Password: string;
  };
  Host: {
    Url: string;
  };
  Monitoring: {
    IntervalMinutes: number;
  };
  Request: {
    FirstRequestFrom: string; // ISO
    FirstRequestTo: string;   // ISO
  };
  Notification: {
    DefaultTemplate: string;
    DefaultEmails: string[];
  };
}

export const DEFAULT_SETTINGS: Settings = {
  Logging: {
    LogLevel: {
      Default: 'Information',
      MicrosoftAspNetCore: 'Warning'
    }
  },
  ConnectionStrings: {
    InternalDb: 'Host=postgres;Port=5432;Database=internal;Username=postgres;Password=postgres',
    ExternalDb: 'Host=postgres;Port=5432;Database=external;Username=postgres;Password=postgres'
  },
  EmailSettings: {
    Host: 'sandbox.smtp.mailtrap.io',
    Port: 2525,
    From: '',
    Username: '',
    Password: ''
  },
  Host: {
    Url: 'http://0.0.0.0:5000'
  },
  Monitoring: {
    IntervalMinutes: 1
  },
  Request: {
    FirstRequestFrom: '2000-01-01T00:00:00',
    FirstRequestTo: '3000-01-01T00:00:00'
  },
  Notification: {
    DefaultTemplate: 'Отсутствует',
    DefaultEmails: []
  }
};
