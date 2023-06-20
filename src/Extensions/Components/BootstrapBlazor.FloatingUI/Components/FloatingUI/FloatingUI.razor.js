import { getDescribedElement } from '../../../BootstrapBlazor/modules/utility.js'

export function init(id) {
    const el = document.getElementById(id)
    const floating = getDescribedElement(el)

    document.body.appendChild(floating)

}

export function dispose(id) {
    console.log('dispose')
}
