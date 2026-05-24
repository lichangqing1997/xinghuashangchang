import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getToken, setToken, removeToken, setUserInfo, removeUserInfo, getUserInfo } from '../utils/storage'
import { login as loginApi, getCurrentUser, logout as logoutApi } from '../api/auth'

interface UserInfo {
  username: string
  role: string
  displayName: string
}

export const useUserStore = defineStore('user', () => {
  const token = ref('')
  const userInfo = ref<UserInfo | null>(null)
  const isLoggedIn = ref(false)

  function loadToken() {
    const saved = getToken()
    if (saved) {
      token.value = saved
      isLoggedIn.value = true
      userInfo.value = getUserInfo<UserInfo>()
    }
  }

  async function login(username: string, password: string) {
    const result = await loginApi({ username, password })
    token.value = result.token
    isLoggedIn.value = true
    setToken(result.token)

    const user = await getCurrentUser()
    userInfo.value = user
    setUserInfo(user)
  }

  async function logout() {
    try {
      await logoutApi()
    } catch {
      // 忽略退出接口错误
    }
    token.value = ''
    isLoggedIn.value = false
    userInfo.value = null
    removeToken()
    removeUserInfo()
  }

  return { token, userInfo, isLoggedIn, loadToken, login, logout }
})
