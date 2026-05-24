import { getToken, removeToken, removeUserInfo } from '../utils/storage'

// H5 环境使用代理，App 环境使用完整地址
// #ifdef H5
const BASE_URL = '/api'
// #endif
// #ifndef H5
const BASE_URL = 'http://192.168.1.3:5242/api'
// #endif

interface Result<T = any> {
  success: boolean
  errorCode?: string
  message: string
  data: T
  timestamp: number
}

interface RequestOptions {
  url: string
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  data?: any
  header?: Record<string, string>
  showLoading?: boolean
}

function request<T = any>(options: RequestOptions): Promise<T> {
  const { url, method = 'GET', data, header = {}, showLoading = true } = options

  if (showLoading) {
    uni.showLoading({ title: '加载中...', mask: true })
  }

  const token = getToken()
  if (token) {
    header['Authorization'] = `Bearer ${token}`
  }
  header['Content-Type'] = header['Content-Type'] || 'application/json'

  return new Promise((resolve, reject) => {
    uni.request({
      url: `${BASE_URL}${url}`,
      method,
      data,
      header,
      success: (res) => {
        if (showLoading) uni.hideLoading()

        if (res.statusCode === 401) {
          removeToken()
          removeUserInfo()
          uni.showToast({ title: '登录已过期，请重新登录', icon: 'none' })
          setTimeout(() => {
            uni.reLaunch({ url: '/pages/login/index' })
          }, 1500)
          reject(new Error('未授权'))
          return
        }

        if (res.statusCode !== 200) {
          uni.showToast({ title: `请求失败(${res.statusCode})`, icon: 'none' })
          reject(new Error(`HTTP ${res.statusCode}`))
          return
        }

        const result = res.data as Result<T>
        if (result.success) {
          resolve(result.data)
        } else {
          uni.showToast({ title: result.message || '操作失败', icon: 'none' })
          reject(new Error(result.message))
        }
      },
      fail: (err) => {
        if (showLoading) uni.hideLoading()
        uni.showToast({ title: '网络请求失败', icon: 'none' })
        reject(err)
      }
    })
  })
}

export default request
export type { Result }
