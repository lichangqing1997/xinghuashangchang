import request from './request'

interface LoginParams {
  username: string
  password: string
}

interface LoginResult {
  token: string
  username: string
  role: string
}

interface UserInfo {
  username: string
  role: string
  displayName: string
}

export function login(data: LoginParams): Promise<LoginResult> {
  return request<LoginResult>({ url: '/Auth/login', method: 'POST', data })
}

export function getCurrentUser(): Promise<UserInfo> {
  return request<UserInfo>({ url: '/Auth/me', method: 'GET' })
}

export function logout(): Promise<void> {
  return request<void>({ url: '/Auth/logout', method: 'POST', showLoading: false })
}
