export interface Platform {
  id: string;
  type: string;
}

export interface PlatformUpsertRequest {
  platform: {
    id?: string;
    type: string;
  };
}
