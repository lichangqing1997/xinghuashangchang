const TOKEN_KEY = 'xinghua_token'
const USER_KEY = 'xinghua_user'

export function getToken(): string {
  return uni.getStorageSync(TOKEN_KEY) || ''
}

export function setToken(token: string) {
  uni.setStorageSync(TOKEN_KEY, token)
}

export function removeToken() {
  uni.removeStorageSync(TOKEN_KEY)
}

export function getUserInfo<T = any>(): T | null {
  const data = uni.getStorageSync(USER_KEY)
  return data ? JSON.parse(data) : null
}

export function setUserInfo(user: any) {
  uni.setStorageSync(USER_KEY, JSON.stringify(user))
}

export function removeUserInfo() {
  uni.removeStorageSync(USER_KEY)
}
