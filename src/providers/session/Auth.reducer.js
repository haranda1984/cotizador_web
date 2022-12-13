import { initialState } from './Auth.provider'

const AuthActions = {
  LOGIN: 'LOGIN',
  LOGOUT: 'LOGOUT'
}

function AuthReducer (state, action) {
  switch (action.type) {
    case AuthActions.LOGIN: {
      const { data } = action
      return { ...state, ...data }
    }
    case AuthActions.LOGOUT: {
      return initialState
    }
    default: {
      return initialState
    }
  }
}

export { AuthActions, AuthReducer }
